using AutoMapper;
using Microsoft.AspNetCore.Http;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.CustomerModel.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.EmailService;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Services.Utils.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace SP25_RPSC.Services.Service.CustomerService
{


    public class CustomerService : ICustomerService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        private readonly IEmailService _emailService;
        private readonly IDecodeTokenHandler _decodeTokenHandler;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler, ICloudinaryStorageService cloudinaryStorageService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryStorageService = cloudinaryStorageService;
            _emailService = emailService;
            _decodeTokenHandler = decodeTokenHandler;

        }

        public async Task<bool> UpdateInfo(UpdateInfoReq model, string email)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(email)
                ?? throw new ApiException(HttpStatusCode.NotFound, "Not Found User");

            var customer = await _unitOfWork.CustomerRepository.GetCustomerByUserId(user.UserId)
                ?? throw new ApiException(HttpStatusCode.BadRequest, "Customer not exists or not active!");

            if (!string.IsNullOrWhiteSpace(model.FullName))
                user.FullName = model.FullName;

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                var existingUser = await _unitOfWork.UserRepository.GetUserByPhoneNumber(model.PhoneNumber);
                if (existingUser != null && existingUser.UserId != user.UserId)
                    throw new ApiException(HttpStatusCode.BadRequest, "Phone number is already in use!");

                user.PhoneNumber = model.PhoneNumber;
            }

            if (model.Avatar != null)
            {
                var avatarList = new List<IFormFile> { model.Avatar };
                var uploadedUrls = await _cloudinaryStorageService.UploadImageAsync(avatarList);
                user.Avatar = uploadedUrls.FirstOrDefault();
            }

            user.Dob = model.Dob ?? user.Dob;
            user.Gender = model.Gender ?? user.Gender;

            customer.Preferences = model.Preferences ?? customer.Preferences;
            customer.LifeStyle = model.LifeStyle ?? customer.LifeStyle;
            customer.BudgetRange = model.BudgetRange ?? customer.BudgetRange;
            customer.PreferredLocation = model.PreferredLocation ?? customer.PreferredLocation;
            customer.Requirement = model.Requirement ?? customer.Requirement;
            customer.CustomerType = model.CustomerType?.ToString() ?? customer.CustomerType;
            customer.Status = StatusEnums.Active.ToString();

            user.UpdateAt = DateTime.UtcNow;

            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();

            return true;
        }


        public async Task<bool> SendRequestRoomSharing(string token, RoomSharingReq request)
        {
            if (token == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var customer = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userId,
                    includeProperties: "User")).FirstOrDefault();
            if (customer == null)
            {
                throw new UnauthorizedAccessException("Customer not found.");
            }

            var postId = request.PostId;
            var post = (await _unitOfWork.PostRepository.Get(filter: p => p.PostId == postId,
                   includeProperties: "RentalRoom,User"
               )).FirstOrDefault();

            if (post == null)
            {
                throw new KeyNotFoundException("Post not found.");
            }
            if (post.Status != StatusEnums.Active.ToString())
            {
                throw new InvalidOperationException("Cannot send request to inactive post.");
            }

            var existingRequest = (await _unitOfWork.RoommateRequestRepository.Get(
                   filter: rr => rr.PostId == postId,
                   includeProperties: "CustomerRequests"
               )).FirstOrDefault();

            if (existingRequest == null)
            {
                existingRequest = new RoommateRequest
                {
                    RequestId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.Now,
                    Status = StatusEnums.Pending.ToString(),
                    PostId = postId
                };
                await _unitOfWork.RoommateRequestRepository.Add(existingRequest);
            }
            else
            {
                var customerAlreadyRequested = existingRequest.CustomerRequests.Any(cr => cr.CustomerId == customer.CustomerId);
                if (customerAlreadyRequested)
                {
                    throw new InvalidOperationException("You have already sent a request for this room.");
                }
            }


            var mess = "Phòng này đẹp quá, cho mình ở ghép với nhé.";
            if (!string.IsNullOrWhiteSpace(request.Message)) { mess = request.Message; }
            var customerRequest = new CustomerRequest
            {
                CustomerRequestId = Guid.NewGuid().ToString(),
                Message = mess,
                Status = StatusEnums.Pending.ToString(),
                RequestId = existingRequest.RequestId,
                CustomerId = customer.CustomerId
            };
            await _unitOfWork.CustomerRequestRepository.Add(customerRequest);

            await _unitOfWork.SaveAsync();

            var postOwnerEmail = post.User.Email;
            var postOwnerName = post.User.FullName;
            var customerName = customer.User.FullName;
            var postTitle = post.Title;
            var roomAddress = post.RentalRoom.Location;

            var roomShareReqMail = EmailTemplate.RoomSharingRequest(postOwnerName, customerName, postTitle, roomAddress, mess);
            await _emailService.SendEmail(postOwnerEmail, "Yêu cầu chia sẻ phòng trọ - ở ghép", roomShareReqMail);

            return true;

        }

        public async Task<ListRequestSharingRes> GetListRequestSharing(string token)
        {
            if (token == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var customer = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userId,
                    includeProperties: "User")).FirstOrDefault();
            if (customer == null)
            {
                throw new UnauthorizedAccessException("Customer not found.");
            }

            var post = (await _unitOfWork.PostRepository.Get(filter: p => p.UserId == userId && p.Status == StatusEnums.Active.ToString(),
                   includeProperties: "RentalRoom,User"
               )).FirstOrDefault();
            if (post == null)
            {
                throw new KeyNotFoundException("NO_POST");
            }

            var roommateRequests = await _unitOfWork.RoommateRequestRepository.Get(
                       filter: r => r.PostId == post.PostId && r.Status == StatusEnums.Pending.ToString(),
                       includeProperties: "CustomerRequests.Customer.User");

            var result = new ListRequestSharingRes();

            foreach (var request in roommateRequests)
            {
                foreach (var customerRequest in request.CustomerRequests)
                {
                    if (customerRequest.Customer != null && customerRequest.Customer.User != null)
                    {
                        var requestInfo = new RequestSharingInfo
                        {
                            RequestId = customerRequest.CustomerRequestId,
                            Message = customerRequest.Message,
                            CustomerType = customerRequest.Customer.CustomerType,
                            Email = customerRequest.Customer.User.Email,
                            FullName = customerRequest.Customer.User.FullName,
                            Dob = customerRequest.Customer.User.Dob,
                            Address = customerRequest.Customer.User.Address,
                            PhoneNumber = customerRequest.Customer.User.PhoneNumber,
                            Gender = customerRequest.Customer.User.Gender,
                            Avatar = customerRequest.Customer.User.Avatar,
                            Preferences = customerRequest.Customer.Preferences,
                            LifeStyle = customerRequest.Customer.LifeStyle,
                            BudgetRange = customerRequest.Customer.BudgetRange,
                            PreferredLocation = customerRequest.Customer.PreferredLocation,
                            Requirement = customerRequest.Customer.Requirement,
                            CustomerId = customerRequest.Customer.CustomerId,
                            UserId = customerRequest.Customer.User.UserId
                        };

                        result.RequestSharingList.Add(requestInfo);
                    }
                }
            }

            result.TotalRequestSharing = result.RequestSharingList.Count;

            return result;
        }

        public async Task<bool> RejectRequestSharing(string token, string requestId)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            if (string.IsNullOrEmpty(requestId))
            {
                throw new ArgumentNullException(nameof(requestId), "Request ID cannot be null or empty.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var customer = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userId,
                    includeProperties: "User")).FirstOrDefault();
            if (customer == null)
            {
                throw new UnauthorizedAccessException("Customer not found.");
            }

            var post = (await _unitOfWork.PostRepository.Get(filter: p => p.UserId == userId && p.Status == StatusEnums.Active.ToString(),
                   includeProperties: "RentalRoom,User"
               )).FirstOrDefault();
            if (post == null)
            {
                throw new KeyNotFoundException("You do not have any active post for looking roommate.");
            }

            var customerRequest = (await _unitOfWork.CustomerRequestRepository.Get(
                                        filter: cr => cr.CustomerRequestId == requestId,
                                        includeProperties: "Request,Customer.User"
                                    )).FirstOrDefault();
            if (customerRequest == null)
            {
                throw new KeyNotFoundException("Request not found.");
            }

            var roommateRequest = customerRequest.Request;
            if (roommateRequest == null || roommateRequest.PostId != post.PostId)
            {
                throw new UnauthorizedAccessException("You are not authorized to reject this request.");
            }

            if (customerRequest.Status != StatusEnums.Pending.ToString())
            {
                throw new InvalidOperationException($"Cannot reject request with status {customerRequest.Status}.");
            }

            customerRequest.Status = StatusEnums.Rejected.ToString();
            await _unitOfWork.CustomerRequestRepository.Update(customerRequest);
            await _unitOfWork.SaveAsync();

            if (customerRequest.Customer?.User != null)
            {
                var requesterEmail = customerRequest.Customer.User.Email;
                var requesterName = customerRequest.Customer.User.FullName ?? "Người dùng";
                var postOwnerName = customer.User.FullName ?? "Chủ phòng";
                var postTitle = post.Title ?? "Phòng trọ";
                var roomAddress = post.RentalRoom?.Location ?? "Không có địa chỉ";

                var rejectRequestMail = EmailTemplate.RoomSharingRejected(
                    requesterName,
                    postOwnerName,
                    postTitle,
                    roomAddress);

                await _emailService.SendEmail(
                    requesterEmail,
                    "Thông báo từ chối yêu cầu chia sẻ phòng trọ",
                    rejectRequestMail);
            }

            return true;
        }

    }
}
