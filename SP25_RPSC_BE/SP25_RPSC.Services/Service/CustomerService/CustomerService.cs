using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.CustomerModel.Response;
using SP25_RPSC.Data.Models.CustomerRentRoomDetail.CustomerRentRoomDetailResponse;
using SP25_RPSC.Data.Repositories.CustomerMoveOutRepository;
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

            var activeRoomStay = (await _unitOfWork.RoomStayCustomerRepository.Get(
            filter: rsc => rsc.CustomerId == customer.CustomerId && rsc.Status == "Active"
                )).FirstOrDefault();

            if (activeRoomStay != null)
            {
                throw new InvalidOperationException("You are already staying in a room. You cannot request to share another room.");
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

        public async Task<bool> CancelRequestRoomSharing(string token, string requestId)
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

            var customerRequest = (await _unitOfWork.CustomerRequestRepository.Get(
                filter: cr => cr.CustomerRequestId == requestId && cr.CustomerId == customer.CustomerId,
                includeProperties: "Request.Post.RentalRoom,Request.Post.User"
            )).FirstOrDefault();

            if (customerRequest == null)
            {
                throw new KeyNotFoundException("Request not found or you don't have permission to cancel it.");
            }

            if (customerRequest.Status != StatusEnums.Pending.ToString())
            {
                throw new InvalidOperationException("Only pending requests can be cancelled.");
            }

            customerRequest.Status = StatusEnums.CANCELLED.ToString();
            await _unitOfWork.CustomerRequestRepository.Update(customerRequest);

            // Check if all customer requests for this roommate request are cancelled
            var allCustomerRequests = (await _unitOfWork.CustomerRequestRepository.Get(
                filter: cr => cr.RequestId == customerRequest.RequestId
            )).ToList();

            var allCancelled = allCustomerRequests.All(cr => cr.Status == StatusEnums.CANCELLED.ToString());

            if (allCancelled)
            {
                var roommateRequest = customerRequest.Request;
                roommateRequest.Status = StatusEnums.CANCELLED.ToString();
                await _unitOfWork.RoommateRequestRepository.Update(roommateRequest);
            }

            await _unitOfWork.SaveAsync();

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
                var filteredCustomerRequests = request.CustomerRequests
                    .Where(c => c.Status == StatusEnums.Pending.ToString())  
                    .ToList();

                foreach (var customerRequest in filteredCustomerRequests)
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
                            Price = post.Price,
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

        public async Task<bool> RejectRequestSharing(string token, RejectSharingReq rejectSharingReq)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var reqId = rejectSharingReq.requestId;
            if (string.IsNullOrEmpty(reqId))
            {
                throw new ArgumentNullException(nameof(reqId), "Request ID cannot be null or empty.");
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
                                        filter: cr => cr.CustomerRequestId == reqId,
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

            var rejectReason = ReasonEnums.RejectSharingReason.ToString();
            if (!string.IsNullOrEmpty(rejectSharingReq.reason)) { rejectReason = rejectSharingReq.reason; }

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
                    roomAddress,
                    rejectReason);

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
                   includeProperties: "RentalRoom,User,RentalRoom.RoomType"
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

            var customerRentRoom = (await _unitOfWork.CustomerRentRoomDetailRequestRepositories.Get(
                                        filter: c => c.CustomerId == customerRequest.CustomerId && c.Status.Equals(StatusEnums.Accepted.ToString())
                                    )).FirstOrDefault();
            var roomStayCus = (await _unitOfWork.RoomStayCustomerRepository.Get(
                                        filter: rs => rs.CustomerId == customerRequest.CustomerId && rs.Status.Equals(StatusEnums.Active.ToString())
                                    )).FirstOrDefault();
            if (customerRentRoom != null && roomStayCus != null)
            {
                throw new InvalidOperationException("This customer maybe has a room, you can not accept he/she to roommate.");
            }

            var rentalRoom = post.RentalRoom;
            var landlord = await _unitOfWork.LandlordRepository.GetLandlordByRentalRoomId(rentalRoom.RoomId);
            var landlordUser = landlord != null ?
                await _unitOfWork.UserRepository.GetUserById(landlord.UserId) : null;

            customerRequest.Status = StatusEnums.Accepted.ToString();
            await _unitOfWork.CustomerRequestRepository.Update(customerRequest);

            //post.Status = StatusEnums.Inactive.ToString();
            //await _unitOfWork.PostRepository.Update(post);

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

                var otherRequestsRentRoomFromSameCustomer = await _unitOfWork.CustomerRentRoomDetailRequestRepositories.Get(
                    filter: cr => cr.CustomerId == acceptedCustomerId
                               && cr.Status == StatusEnums.Pending.ToString());

                foreach (var otherRentRoomRequest in otherRequestsRentRoomFromSameCustomer)
                {
                    otherRentRoomRequest.Status = StatusEnums.Inactive.ToString();
                    await _unitOfWork.CustomerRentRoomDetailRequestRepositories.Update(otherRentRoomRequest);
                }
            }

            var roomStay = (await _unitOfWork.RoomStayRepository.Get(
                        filter: rs => rs.RoomId == rentalRoom.RoomId && rs.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
            if (roomStay == null)
            {
                throw new KeyNotFoundException("Room stay not found.");
            }

            var roomStayCusNew = new RoomStayCustomer
            {
                RoomStayCustomerId = Guid.NewGuid().ToString(),
                Type = CustomerTypeEnums.Member.ToString(),
                Status = StatusEnums.Active.ToString(),
                RoomStayId = roomStay.RoomStayId,
                CustomerId = acceptedCustomerId,
                UpdatedAt = null,
                LandlordId = landlord.LandlordId
            };
            await _unitOfWork.RoomStayCustomerRepository.Add(roomStayCusNew);
            await _unitOfWork.SaveAsync();

            /*
            var otherRequests = await _unitOfWork.CustomerRequestRepository.Get(
                filter: cr => cr.RequestId == roommateRequest.RequestId && cr.CustomerRequestId != requestId && cr.Status == StatusEnums.Pending.ToString(),
                includeProperties: "Customer.User");

            var rejectReason = "Người đăng bài đã tìm được bạn ở ghép phù hợp.";

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
                        addressRoom,
                        reason: rejectReason
                        );

                    await _emailService.SendEmail(
                        rejectedRequesterEmail,
                        "Thông báo từ chối yêu cầu chia sẻ phòng trọ",
                        rejectMailContent);
                }
            }

            roommateRequest.Status = StatusEnums.Completed.ToString();
            await _unitOfWork.RoommateRequestRepository.Update(roommateRequest);
            await _unitOfWork.SaveAsync();
            */

            var currentMemberCount = await _unitOfWork.RoomStayCustomerRepository.Get(
                        filter: rsc => rsc.RoomStayId == roomStay.RoomStayId && rsc.Status == StatusEnums.Active.ToString()
                    );

            int maxCapacity = rentalRoom.RoomType?.MaxOccupancy ?? 0;
            if (currentMemberCount.Count() >= maxCapacity)
            {
                post.Status = StatusEnums.Inactive.ToString();
                await _unitOfWork.PostRepository.Update(post);

                roommateRequest.Status = StatusEnums.Completed.ToString();
                await _unitOfWork.RoommateRequestRepository.Update(roommateRequest);
                await _unitOfWork.SaveAsync();

                var remainingRequests = await _unitOfWork.CustomerRequestRepository.Get(
                    filter: cr => cr.RequestId == roommateRequest.RequestId
                              && cr.Status == StatusEnums.Pending.ToString(),
                    includeProperties: "Customer.User");

                foreach (var remainingRequest in remainingRequests)
                {
                    remainingRequest.Status = StatusEnums.Rejected.ToString();
                    await _unitOfWork.CustomerRequestRepository.Update(remainingRequest);

                    if (remainingRequest.Customer?.User != null)
                    {
                        var rejectedEmail = remainingRequest.Customer.User.Email;
                        var rejectedName = remainingRequest.Customer.User.FullName ?? "Người dùng";
                        var ownerName = customer.User.FullName ?? "Chủ phòng";
                        var titlePost = post.Title ?? "Phòng trọ";
                        var addressRoom = post.RentalRoom?.Location ?? "Không có địa chỉ";
                        var rejectReason = "Phòng trọ đã đủ người ở ghép.";

                        var rejectContent = EmailTemplate.RoomSharingRejected(
                            rejectedName,
                            ownerName,
                            titlePost,
                            addressRoom,
                            reason: rejectReason
                        );

                        await _emailService.SendEmail(
                            rejectedEmail,
                            "Thông báo từ chối yêu cầu chia sẻ phòng trọ",
                            rejectContent);
                    }
                }

                await _unitOfWork.SaveAsync();
            }

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

        public async Task<bool> RequestLeaveRoom(string token)
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

            var roomStayCustomer = (await _unitOfWork.RoomStayCustomerRepository.Get(filter: rsc => rsc.Customer.CustomerId == customer.CustomerId
                    && rsc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
            if (roomStayCustomer == null)
            {
                throw new KeyNotFoundException("You do not in any room.");
            }
            if (!roomStayCustomer.Type.Equals(CustomerTypeEnums.Member.ToString()))
            {
                throw new UnauthorizedAccessException("This is request leave room API for member!");
            }

            var existingMoveOutRequest = await _unitOfWork.CustomerMoveOutRepository.Get(
                    filter: cmo => cmo.UserMoveId == userId &&
                                  cmo.RoomStayId == roomStayCustomer.RoomStayId &&
                                  cmo.Status == 0);

            if (existingMoveOutRequest.Any())
            {
                throw new InvalidOperationException("You have already send a request to leave this room.");
            }

            var roomStay = await _unitOfWork.RoomStayRepository.GetByIDAsync(roomStayCustomer.RoomStayId);
            if (roomStay == null)
            {
                throw new KeyNotFoundException("Room stay information not found.");
            }


            var customerMoveOut = new CustomerMoveOut
            {
                Cmoid = Guid.NewGuid().ToString(),
                UserMoveId = userId,
                UserDepositeId = null,
                RoomStayId = roomStay.RoomStayId,
                DateRequest = DateTime.Now,
                Status = 0
            };

            await _unitOfWork.CustomerMoveOutRepository.Add(customerMoveOut);
            await _unitOfWork.SaveAsync();

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.LandlordId == roomStay.LandlordId,
                    includeProperties: "User")).FirstOrDefault(); 
            if (landlord == null)
            {
                throw new KeyNotFoundException("Landlord not found.");
            }

            var tenant = (await _unitOfWork.RoomStayCustomerRepository.Get(
                    filter: rsc => rsc.RoomStayId == roomStay.RoomStayId &&
                                  rsc.Status.Equals(StatusEnums.Active.ToString()) &&
                                  rsc.Type.Equals(CustomerTypeEnums.Tenant.ToString()) && 
                                  rsc.CustomerId != customer.CustomerId, 
                    includeProperties: "Customer,Customer.User")).FirstOrDefault();
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var room = await _unitOfWork.RoomRepository.GetByIDAsync(roomStay.RoomId);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            var landlordMail = landlord.User.Email;
            var landlordName = landlord.User.FullName ?? "Chủ phòng";
            var tenantdMail = tenant.Customer.User.Email;
            var tenantName = tenant.Customer.User.FullName ?? "Người chịu trách nhiệm";
            var memberName = customer.User.FullName ?? "Thành viên";
            var roomNumber = room.RoomNumber ?? "Số phòng";
            var roomAddress = room.Location ?? "Địa chỉ";
            var dateRequest = DateTime.Now.ToString("dd/MM/yyyy");

            var mailLeaveRoomForLandlord = EmailTemplate.MemberLeaveRoomNotificationForLandlord(
                landlordName,
                tenantName,
                memberName,
                roomNumber,
                roomAddress,
                dateRequest.ToString());

            await _emailService.SendEmail(
                landlordMail,
                "Thông báo yêu cầu rời phòng",
                mailLeaveRoomForLandlord);

            var mailLeaveRoomForTenant = EmailTemplate.MemberLeaveRoomNotificationForTenant(
                tenantName,
                memberName,
                roomNumber,
                roomAddress,
                dateRequest.ToString());

            await _emailService.SendEmail(
                tenantdMail,
                "Thông báo yêu cầu rời phòng",
                mailLeaveRoomForTenant);

            var mailLeaveRoomConfirmForMember = EmailTemplate.LeaveRoomConfirmationForMember(
                memberName,
                tenantName,
                roomNumber,
                roomAddress,
                dateRequest.ToString());

            await _emailService.SendEmail(
                customer.User.Email,
                "Thông báo xác nhận yêu cầu rời phòng",
                mailLeaveRoomConfirmForMember);

            return true;
        }

        public async Task<ListLeaveRoomRes> GetListLeaveRoomRequest(string token)
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

            var roomStayCustomer = (await _unitOfWork.RoomStayCustomerRepository.Get(filter: rsc => rsc.Customer.CustomerId == customer.CustomerId
                    && rsc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
            if (roomStayCustomer == null)
            {
                throw new KeyNotFoundException("You do not in any room.");
            }
            if (!roomStayCustomer.Type.Equals(CustomerTypeEnums.Tenant.ToString()))
            {
                throw new UnauthorizedAccessException("This is get list room leaving request API for tenant!");
            }

            var roomStay = await _unitOfWork.RoomStayRepository.GetByIDAsync(roomStayCustomer.RoomStayId);
            if (roomStay == null)
            {
                throw new KeyNotFoundException("Room stay information not found.");
            }

            var leaveRoomListRequest = await _unitOfWork.CustomerMoveOutRepository.Get(
                    filter: cmo => cmo.RoomStayId == roomStayCustomer.RoomStayId && cmo.Status == 0
                    && cmo.UserMoveId != userId,
                    includeProperties: "UserMove");
            if (leaveRoomListRequest == null || !leaveRoomListRequest.Any())
            {
                throw new KeyNotFoundException("No room leaving request.");
            }

            var result = new ListLeaveRoomRes();

            foreach (var request in leaveRoomListRequest)
            {
                if (request.UserMove != null)
                {
                    var userMove = request.UserMove;
                    var memberCustomer = await _unitOfWork.CustomerRepository.Get(
                        filter: c => c.UserId == userMove.UserId,
                        includeProperties: "User");

                    var memberInfo = memberCustomer.FirstOrDefault();

                    if (memberInfo != null && memberInfo.User != null)
                    {
                        var customerMoveOutRes = new CustomerMoveOutRes
                        {
                            Cmoid = request.Cmoid,
                            UserMoveId = request.UserMoveId,
                            RoomStayId = request.RoomStayId,
                            DateRequest = request.DateRequest,
                            Status = request.Status,
                            MemberInfo = new MemberInfo
                            {
                                UserId = userMove.UserId,
                                CustomerId = memberInfo.CustomerId,
                                Email = memberInfo.User.Email,
                                FullName = memberInfo.User.FullName,
                                Dob = memberInfo.User.Dob,
                                PhoneNumber = memberInfo.User.PhoneNumber,
                                Gender = memberInfo.User.Gender,
                                Avatar = memberInfo.User.Avatar
                            }
                        };

                        result.LeaveRoomRequestList.Add(customerMoveOutRes);
                    }
                }
            }
            return result;
        }

        public async Task<bool> AcceptLeaveRoomRequest(string token, string requestId)
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

            var roomStayCustomer = (await _unitOfWork.RoomStayCustomerRepository.Get(filter: rsc => rsc.Customer.CustomerId == customer.CustomerId
                    && rsc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
            if (roomStayCustomer == null)
            {
                throw new KeyNotFoundException("You do not in any room.");
            }
            if (!roomStayCustomer.Type.Equals(CustomerTypeEnums.Tenant.ToString()))
            {
                throw new UnauthorizedAccessException("This is accept room leaving request API for tenant!");
            }

            var roomStay = await _unitOfWork.RoomStayRepository.GetByIDAsync(roomStayCustomer.RoomStayId);
            if (roomStay == null)
            {
                throw new KeyNotFoundException("Room stay information not found.");
            }

            var customerMoveOut = await _unitOfWork.CustomerMoveOutRepository.GetByIDAsync(requestId);
            if (customerMoveOut == null || customerMoveOut.Status == 1)
            {
                throw new KeyNotFoundException("Room leaving request not found.");
            }
            if (customerMoveOut.RoomStayId != roomStayCustomer.RoomStayId)
            {
                throw new UnauthorizedAccessException("You are not in the same room.");
            }

            var userMoveId = customerMoveOut.UserMoveId;
            var cusMove = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userMoveId,
                    includeProperties: "User")).FirstOrDefault();

            customerMoveOut.Status = 1;
            await _unitOfWork.CustomerMoveOutRepository.Update(customerMoveOut);

            var roomStayCustomerMove = (await _unitOfWork.RoomStayCustomerRepository.Get(filter: rsc => rsc.Customer.CustomerId == cusMove.CustomerId
                    && rsc.RoomStayId == roomStayCustomer.RoomStayId
                    && rsc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
            if (roomStayCustomerMove == null)
            {
                throw new KeyNotFoundException("Customer not in this room or already leave room.");
            }
            roomStayCustomerMove.Status = StatusEnums.Inactive.ToString();
            await _unitOfWork.RoomStayCustomerRepository.Update(roomStayCustomerMove);

            var room = await _unitOfWork.RoomRepository.GetByIDAsync(roomStay.RoomId);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.LandlordId == roomStay.LandlordId,
                    includeProperties: "User")).FirstOrDefault();
            if (landlord == null)
            {
                throw new KeyNotFoundException("Landlord not found.");
            }

            var mailLeaveRoomAcceptedForLandlord = EmailTemplate.LeaveRoomAcceptedNotificationForLandlord(
                landlord.User.FullName,
                cusMove.User.FullName,     
                customer.User.FullName,    
                room.RoomNumber,           
                room.Location,             
                DateTime.Now.ToString("dd/MM/yyyy")  
            );

            var landlordMail = landlord.User.Email;
            await _emailService.SendEmail(
                landlordMail,
                "Thông báo yêu cầu rời phòng của thành viên được xác nhận",
                mailLeaveRoomAcceptedForLandlord);


            var mailLeaveRoomAcceptedForMember = EmailTemplate.LeaveRoomAcceptedNotificationForMember(
                cusMove.User.FullName, 
                customer.User.FullName, 
                room.RoomNumber,
                room.Location,       
                DateTime.Now.ToString("dd/MM/yyyy")  
            );

            var cusMoveMail = cusMove.User.Email;
            await _emailService.SendEmail(
                cusMoveMail,
                "Thông báo yêu cầu rời phòng của bạn đã được xác nhận",
                mailLeaveRoomAcceptedForMember
            );

            await _unitOfWork.SaveAsync();
            return true;
        }


        public async Task<bool> CheckLeaveRoomRequest(string userId, string roomStayId)
        {
            var leaveRoomListRequest = await _unitOfWork.CustomerMoveOutRepository.Get(
                    filter: cmo => cmo.RoomStayId == roomStayId && cmo.Status == 0
                    && cmo.UserMoveId != userId,
                    includeProperties: "UserMove");
            if (leaveRoomListRequest == null || !leaveRoomListRequest.Any())
            {
                return false;
            }
            return true;
        }

        public async Task<bool> RequestLeaveRoomByTenant(string token, TenantRoomLeavingReq request)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            // Verify if the user is a customer
            var customer = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userId,
                    includeProperties: "User")).FirstOrDefault();
            if (customer == null)
            {
                throw new UnauthorizedAccessException("Customer not found.");
            }

            // Check if the customer is a tenant in an active room stay
            var roomStayCustomer = (await _unitOfWork.RoomStayCustomerRepository.Get(filter: rsc => rsc.Customer.CustomerId == customer.CustomerId
                    && rsc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
            if (roomStayCustomer == null)
            {
                throw new KeyNotFoundException("You are not in any room.");
            }
            if (!roomStayCustomer.Type.Equals(CustomerTypeEnums.Tenant.ToString()))
            {
                throw new UnauthorizedAccessException("This is a request leave room API for tenants only!");
            }

            // Check if there's already a pending request
            var existingMoveOutRequest = await _unitOfWork.CustomerMoveOutRepository.Get(
                    filter: cmo => cmo.UserMoveId == userId &&
                                  cmo.RoomStayId == roomStayCustomer.RoomStayId &&
                                  cmo.Status == 0);

            if (existingMoveOutRequest.Any())
            {
                throw new InvalidOperationException("You have already sent a request to leave this room.");
            }

            // Get the room stay information
            var roomStay = await _unitOfWork.RoomStayRepository.GetByIDAsync(roomStayCustomer.RoomStayId);
            if (roomStay == null)
            {
                throw new KeyNotFoundException("Room stay information not found.");
            }

            var checkLeaveRoomReq = await CheckLeaveRoomRequest(userId, roomStay.RoomStayId);
            if (checkLeaveRoomReq)
            {
                throw new ArgumentException("You must process your roommate's request to leave first.");
            }

            // Get all active members in the room
            var activeMembers = await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rsc => rsc.RoomStayId == roomStay.RoomStayId &&
                              rsc.Status.Equals(StatusEnums.Active.ToString()) &&
                              rsc.CustomerId != customer.CustomerId,
                includeProperties: "Customer,Customer.User");

            if (activeMembers.Any())
            {
                // Tenant must designate a new person in charge (userDepositeId)
                if (string.IsNullOrEmpty(request.userDepositeId))
                {
                    throw new ArgumentException("You must designate a new person in charge before leaving the room.");
                }

                // Verify if the designated person exists and is a member of the room
                var designatedMember = activeMembers.FirstOrDefault(m => m.Customer.UserId == request.userDepositeId);
                if (designatedMember == null)
                {
                    throw new KeyNotFoundException("The designated person is not a member of this room.");
                }

                // Check if the designated person is already a tenant
                if (designatedMember.Type.Equals(CustomerTypeEnums.Tenant.ToString()))
                {
                    throw new InvalidOperationException("The designated person is already a tenant.");
                }
            } else {
                // No need to designate someone if tenant is the only person in the room
                request.userDepositeId = null;
            }

            var customerMoveOut = new CustomerMoveOut
            {
                Cmoid = Guid.NewGuid().ToString(),
                UserMoveId = userId,
                UserDepositeId = request.userDepositeId,
                RoomStayId = roomStay.RoomStayId,
                DateRequest = DateTime.Now,
                Status = 0
            };
            await _unitOfWork.CustomerMoveOutRepository.Add(customerMoveOut);
            await _unitOfWork.SaveAsync();

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.LandlordId == roomStay.LandlordId,
                            includeProperties: "User")).FirstOrDefault();
            if (landlord == null)
            {
                throw new KeyNotFoundException("Landlord not found.");
            }

            var room = await _unitOfWork.RoomRepository.GetByIDAsync(roomStay.RoomId);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            var tenantName = customer.User.FullName ?? "Người chịu trách nhiệm";
            var roomNumber = room.RoomNumber ?? "Số phòng";
            var roomAddress = room.Location ?? "Địa chỉ";
            var dateRequest = DateTime.Now.ToString("dd/MM/yyyy");

            string designatedUserName = "Không có";
            if (!string.IsNullOrEmpty(request.userDepositeId))
            {
                var designatedUser = (await _unitOfWork.CustomerRepository.Get(
                    filter: c => c.UserId == request.userDepositeId,
                    includeProperties: "User")).FirstOrDefault();

                if (designatedUser != null && designatedUser.User != null)
                {
                    designatedUserName = designatedUser.User.FullName ?? "Không có tên";
                }
            }

            // Send email to landlord
            var mailLeaveRoomForLandlord = EmailTemplate.TenantLeaveRoomNotificationForLandlord(
                landlord.User.FullName ?? "Chủ phòng",
                tenantName,
                roomNumber,
                roomAddress,
                dateRequest,
                designatedUserName);

            await _emailService.SendEmail(
                landlord.User.Email,
                "Thông báo yêu cầu rời phòng từ người thuê chính",
                mailLeaveRoomForLandlord);

            // Send confirmation email to the tenant
            var mailLeaveRoomConfirmForTenant = EmailTemplate.LeaveRoomConfirmationForTenant(
                tenantName,
                roomNumber,
                roomAddress,
                dateRequest,
                designatedUserName);

            await _emailService.SendEmail(
                customer.User.Email,
                "Thông báo xác nhận yêu cầu rời phòng",
                mailLeaveRoomConfirmForTenant);

            // Send notification emails to all other members in the room
            foreach (var member in activeMembers)
            {
                var memberName = member.Customer.User.FullName ?? "Thành viên";
                var mailLeaveRoomForMember = EmailTemplate.TenantLeaveRoomNotificationForMembers(
                    memberName,
                    tenantName,
                    roomNumber,
                    roomAddress,
                    dateRequest,
                    designatedUserName,
                    member.Customer.UserId == request.userDepositeId);

                await _emailService.SendEmail(
                    member.Customer.User.Email,
                    "Thông báo yêu cầu rời phòng từ người thuê chính",
                    mailLeaveRoomForMember);
            }

            return true;
        }

        public async Task<bool> KickRoommateByTenant(string token, KickRoommateReq kickRoommateReq)
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

            var roomStayCustomer = (await _unitOfWork.RoomStayCustomerRepository.Get(filter: rsc => rsc.Customer.CustomerId == customer.CustomerId
                    && rsc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
            if (roomStayCustomer == null)
            {
                throw new KeyNotFoundException("You do not in any room.");
            }
            if (!roomStayCustomer.Type.Equals(CustomerTypeEnums.Tenant.ToString()))
            {
                throw new UnauthorizedAccessException("This is kick roommate out of room API for tenant!");
            }

            var roomStay = await _unitOfWork.RoomStayRepository.GetByIDAsync(roomStayCustomer.RoomStayId);
            if (roomStay == null)
            {
                throw new KeyNotFoundException("Room stay information not found.");
            }

            var room = await _unitOfWork.RoomRepository.GetByIDAsync(roomStay.RoomId);
            if (room == null)
            {
                throw new KeyNotFoundException("Room information not found.");
            }

            var memberKickedId = kickRoommateReq.customerId;
            if (string.IsNullOrEmpty(memberKickedId))
            {
                throw new KeyNotFoundException("Customer Id is requied!");
            }

            var memberKicked = (await _unitOfWork.CustomerRepository.Get(filter: c => c.CustomerId == memberKickedId,
                    includeProperties: "User")).FirstOrDefault();
            if (memberKickedId == null)
            {
                throw new UnauthorizedAccessException("The selected member kick not found.");
            }

            var checkMemberKicked = (await _unitOfWork.RoomStayCustomerRepository.Get(
                    filter: c => c.CustomerId == memberKickedId
                            && c.RoomStayId == roomStay.RoomStayId
                            && c.Status.Equals(StatusEnums.Active.ToString())
                            && c.Type.Equals(CustomerTypeEnums.Member.ToString()),
                    includeProperties: "Customer,Customer.User")).FirstOrDefault();
            if (checkMemberKicked == null)
            {
                throw new KeyNotFoundException("The selected member is not in your room or they have left.");
            }

            List<string> evidenceImageUrls = new List<string>();
            if (kickRoommateReq.evidenceImages != null && kickRoommateReq.evidenceImages.Any())
            {
                try
                {
                    evidenceImageUrls = await _cloudinaryStorageService.UploadImageAsync(kickRoommateReq.evidenceImages);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to upload evidence images: {ex.Message}");
                }
            }

            checkMemberKicked.Status = StatusEnums.Inactive.ToString();
            await _unitOfWork.RoomStayCustomerRepository.Update(checkMemberKicked);
            await _unitOfWork.SaveAsync();

            var kickReason = ReasonEnums.KickRoommateReason.ToString();
            if (!string.IsNullOrEmpty(kickRoommateReq.reason)) { kickReason = kickRoommateReq.reason; }

            if (checkMemberKicked.Customer?.User != null)
            {
                var requesterEmail = checkMemberKicked.Customer.User.Email;
                var roommateName = checkMemberKicked.Customer.User.FullName ?? "";
                var tenantName = customer.User.FullName ?? "Người chịu trách nhiệm chính";
                var roomTitle = room.Title ?? "Phòng trọ";
                var roomNumbser = room.RoomNumber ?? "";
                var roomAddress = room.Location ?? "Không có địa chỉ";
                var date = DateTime.Now.ToString("dd/MM/yyyy");

                var kickRoommateMail = EmailTemplate.RoommateKicked(
                    roommateName,
                    tenantName,
                    roomTitle,
                    roomNumbser,
                    roomAddress,
                    kickReason,
                    date);

                await _emailService.SendEmail(
                    requesterEmail,
                    "Thông báo chấm dứt ở ghép",
                    kickRoommateMail);
            }


            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.LandlordId == roomStay.LandlordId,
                    includeProperties: "User")).FirstOrDefault();
            if (landlord != null && landlord.User != null)
            {
                var landlordEmail = landlord.User.Email;
                var landlordName = landlord.User.FullName ?? "Chủ nhà";
                var roommateName = checkMemberKicked.Customer?.User?.FullName ?? "Thành viên";
                var tenantName = customer.User.FullName ?? "Người chịu trách nhiệm chính";
                var roomTitle = room.Title ?? "Phòng trọ";
                var roomNumber = room.RoomNumber ?? "";
                var roomAddress = room.Location ?? "Không có địa chỉ";
                var date = DateTime.Now.ToString("dd/MM/yyyy");

                var landlordNotificationMail = EmailTemplate.RoommateKickedLandlordNoti(
                    landlordName,
                    tenantName,
                    roommateName,
                    roomTitle,
                    roomNumber,
                    roomAddress,
                    kickReason,
                    date,
                    evidenceImageUrls);

                await _emailService.SendEmail(
                    landlordEmail,
                    "Thông báo: Thành viên kết thúc ở ghép",
                    landlordNotificationMail);
            }

            return true;
        }

        public async Task<List<PastRoommateRes>> GetPastRoommates(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var currentCustomer = (await _unitOfWork.CustomerRepository.Get(
                filter: c => c.UserId == userId,
                includeProperties: "User"
            )).FirstOrDefault();

            if (currentCustomer == null)
            {
                throw new UnauthorizedAccessException("Customer not found.");
            }

            var customerRoomStays = await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rsc => rsc.CustomerId == currentCustomer.CustomerId,
                includeProperties: "RoomStay"
            );

            var roomStayIds = customerRoomStays.Select(rsc => rsc.RoomStayId).ToList();

            var pastRoommates = new List<Customer>();

            foreach (var roomStayId in roomStayIds)
            {
                var roommatesInRoomStay = await _unitOfWork.RoomStayCustomerRepository.Get(
                    filter: rsc => rsc.RoomStayId == roomStayId && rsc.CustomerId != currentCustomer.CustomerId,
                    includeProperties: "Customer,Customer.User"
                );

                foreach (var roommate in roommatesInRoomStay)
                {
                    if (roommate.Customer != null && !pastRoommates.Any(c => c.CustomerId == roommate.Customer.CustomerId))
                    {
                        pastRoommates.Add(roommate.Customer);
                    }
                }
            }

            var currentRoomStays = customerRoomStays
                .Where(rsc => rsc.Status.Equals(StatusEnums.Active.ToString()) || rsc.Status.Equals(StatusEnums.Pending.ToString()))
                .Select(rsc => rsc.RoomStayId)
                .ToList();

            var currentRoommates = new List<string>();

            foreach (var roomStayId in currentRoomStays)
            {
                var roommatesInRoomStay = await _unitOfWork.RoomStayCustomerRepository.Get(
                    filter: rsc => rsc.RoomStayId == roomStayId &&
                                   rsc.CustomerId != currentCustomer.CustomerId &&
                                   (rsc.Status.Equals(StatusEnums.Active.ToString()) || rsc.Status.Equals(StatusEnums.Pending.ToString()))
                );

                currentRoommates.AddRange(roommatesInRoomStay.Select(r => r.CustomerId).ToList());
            }

            var pastRoommatesFiltered = pastRoommates
                .Where(pr => !currentRoommates.Contains(pr.CustomerId))
                .ToList();

            var response = pastRoommatesFiltered.Select(pr => new PastRoommateRes
            {
                CustomerId = pr.CustomerId,
                UserId = pr.UserId,
                FullName = pr.User?.FullName,
                Email = pr.User?.Email,
                PhoneNumber = pr.User?.PhoneNumber,
                AvatarUrl = pr.User?.Avatar,
            }).ToList();

            return response;
        }
    }
}
