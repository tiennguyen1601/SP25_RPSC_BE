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

        public async Task<ListSentRequestSharingRes> GetListSentRequestSharing(string token)
        {
            if (string.IsNullOrEmpty(token))
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

            var customerRequests = await _unitOfWork.CustomerRequestRepository.Get(
                filter: cr => cr.CustomerId == customer.CustomerId,
                includeProperties: "Request.Post.RentalRoom,Request.Post.User");

            var result = new ListSentRequestSharingRes
            {
                SentRequestSharingList = new List<SentRequestSharingInfo>()
            };

            foreach (var request in customerRequests)
            {
                if (request.Request?.Post != null)
                {
                    var post = request.Request.Post;
                    var postOwner = post.User;

                    var requestInfo = new SentRequestSharingInfo
                    {
                        RequestId = request.CustomerRequestId,
                        Message = request.Message,
                        Status = request.Status,
                        CreatedAt = request.Request.CreatedAt,
                        PostInfo = new PostInfoForRequest
                        {
                            PostId = post.PostId,
                            Title = post.Title,
                            Description = post.Description,
                            Location = post.RentalRoom?.Location,
                            PostOwnerName = postOwner?.FullName,
                            PostOwnerAvatar = postOwner?.Avatar,
                            PostOwnerPhone = postOwner?.PhoneNumber,
                            PostOwnerEmail = postOwner?.Email
                        }
                    };

                    result.SentRequestSharingList.Add(requestInfo);
                }
            }

            result.TotalSentRequests = result.SentRequestSharingList.Count;

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

        public async Task<bool> AcceptRequestSharing(string token, string requestId)
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
                throw new UnauthorizedAccessException("You are not authorized to accept this request.");
            }

            if (customerRequest.Status != StatusEnums.Pending.ToString())
            {
                throw new InvalidOperationException($"Cannot accept request with status {customerRequest.Status}.");
            }

            var rentalRoom = post.RentalRoom;
            var landlord = await _unitOfWork.LandlordRepository.GetLandlordByRentalRoomId(rentalRoom.RoomId);
            var landlordUser = landlord != null ?
                await _unitOfWork.UserRepository.GetUserById(landlord.UserId) : null;

            customerRequest.Status = StatusEnums.Accepted.ToString();
            await _unitOfWork.CustomerRequestRepository.Update(customerRequest);

            post.Status = StatusEnums.Inactive.ToString();
            await _unitOfWork.PostRepository.Update(post);

            var acceptedCustomerId = customerRequest.CustomerId;
            if (!string.IsNullOrEmpty(acceptedCustomerId))
            {
                var otherRequestsFromSameCustomer = await _unitOfWork.CustomerRequestRepository.Get(
                    filter: cr => cr.CustomerId == acceptedCustomerId
                               && cr.CustomerRequestId != requestId
                               && cr.Status == StatusEnums.Pending.ToString(),
                    includeProperties: "Request.Post.RentalRoom,Request.Post.User");

                foreach (var otherRequest in otherRequestsFromSameCustomer)
                {
                    otherRequest.Status = StatusEnums.Inactive.ToString();
                    await _unitOfWork.CustomerRequestRepository.Update(otherRequest);
                }
            }

            var otherRequests = await _unitOfWork.CustomerRequestRepository.Get(
                filter: cr => cr.RequestId == roommateRequest.RequestId && cr.CustomerRequestId != requestId && cr.Status == StatusEnums.Pending.ToString(),
                includeProperties: "Customer.User");

            foreach (var otherRequest in otherRequests)
            {
                otherRequest.Status = StatusEnums.Rejected.ToString();
                await _unitOfWork.CustomerRequestRepository.Update(otherRequest);

                if (otherRequest.Customer?.User != null)
                {
                    var rejectedRequesterEmail = otherRequest.Customer.User.Email;
                    var rejectedRequesterName = otherRequest.Customer.User.FullName ?? "Người dùng";
                    var ownerPostName = customer.User.FullName ?? "Chủ phòng";
                    var titlePost = post.Title ?? "Phòng trọ";
                    var addressRoom = post.RentalRoom?.Location ?? "Không có địa chỉ";

                    var rejectMailContent = EmailTemplate.RoomSharingRejected(
                        rejectedRequesterName,
                        ownerPostName,
                        titlePost,
                        addressRoom);

                    await _emailService.SendEmail(
                        rejectedRequesterEmail,
                        "Thông báo từ chối yêu cầu chia sẻ phòng trọ",
                        rejectMailContent);
                }
            }

            roommateRequest.Status = StatusEnums.Completed.ToString();
            await _unitOfWork.RoommateRequestRepository.Update(roommateRequest);
            await _unitOfWork.SaveAsync();

            var acceptedRequesterEmail = customerRequest.Customer.User.Email;
            var acceptedRequesterName = customerRequest.Customer.User.FullName ?? "Người dùng";
            var postOwnerName = customer.User.FullName ?? "Chủ phòng";
            var postTitle = post.Title ?? "Phòng trọ";
            var roomAddress = post.RentalRoom?.Location ?? "Không có địa chỉ";
            var postOwnerPhone = customer.User.PhoneNumber ?? "Không có số điện thoại";
            var postOwnerEmail = customer.User.Email ?? "Không có email";

            var acceptMailContent = EmailTemplate.RoomSharingAccepted(
                acceptedRequesterName,
                postOwnerName,
                postTitle,
                roomAddress,
                postOwnerPhone,
                postOwnerEmail);

            await _emailService.SendEmail(
                acceptedRequesterEmail,
                "Chúc mừng! Yêu cầu chia sẻ phòng trọ của bạn đã được chấp nhận",
                acceptMailContent);

            var confirmationMailContent = EmailTemplate.RoomSharingConfirmation(
                postOwnerName,
                acceptedRequesterName,
                postTitle,
                roomAddress,
                customerRequest.Customer.User.PhoneNumber ?? "Không có số điện thoại",
                customerRequest.Customer.User.Email ?? "Không có email");

            await _emailService.SendEmail(
                customer.User.Email,
                "Xác nhận chấp nhận yêu cầu chia sẻ phòng trọ",
                confirmationMailContent);

            if (landlordUser != null)
            {
                var landlordMailContent = EmailTemplate.RoomSharingNotificationToLandlord(
                    landlordUser.FullName ?? "Chủ nhà",
                    postOwnerName,
                    acceptedRequesterName,
                    postTitle,
                    roomAddress);

                await _emailService.SendEmail(
                    landlordUser.Email,
                    "Thông báo phòng trọ đã được chia sẻ thành công",
                    landlordMailContent);
            }

            return true;
        }

    }
}
