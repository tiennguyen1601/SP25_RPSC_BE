using AutoMapper;
using Microsoft.AspNetCore.Http;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.CustomerModel.Request;
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


    }
}
