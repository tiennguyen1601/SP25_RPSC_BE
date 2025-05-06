using AutoMapper;
using CloudinaryDotNet.Actions;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.CustomerModel.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.EmailService;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Services.Utils.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.LandlordService
{
    public class LandlordService : ILandlordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        private readonly IEmailService _emailService;
        private readonly IDecodeTokenHandler _decodeTokenHandler;

        public LandlordService(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler, ICloudinaryStorageService cloudinaryStorageService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryStorageService = cloudinaryStorageService;
            _emailService = emailService;
            _decodeTokenHandler = decodeTokenHandler;

        }


        public async Task<Landlord?> GetLandlordById(string id)
        {
            return await _unitOfWork.LandlordRepository.GetAsync(id);
        }

        public async Task<ListTenantLeaveRoomRes> GetListTenantLeaveRoomRequest(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId,
                    includeProperties: "User")).FirstOrDefault();

            if (landlord == null)
            {
                throw new UnauthorizedAccessException("Landlord not found.");
            }

            // Get all roomstays associated with this landlord
            var roomStays = await _unitOfWork.RoomStayRepository.Get(
                filter: rs => rs.LandlordId == landlord.LandlordId && rs.Status.Equals(StatusEnums.Active.ToString()),
                includeProperties: "Room");

            if (roomStays == null || !roomStays.Any())
            {
                throw new KeyNotFoundException("No active room stays found for this landlord.");
            }

            var result = new ListTenantLeaveRoomRes();

            foreach (var roomStay in roomStays)
            {
                // Get all pending leave requests for this roomstay
                var leaveRequests = await _unitOfWork.CustomerMoveOutRepository.Get(
                    filter: cmo => cmo.RoomStayId == roomStay.RoomStayId && cmo.Status == 0,
                    includeProperties: "UserMove,UserDeposite");

                if (leaveRequests != null && leaveRequests.Any())
                {
                    foreach (var request in leaveRequests)
                    {
                        if (request.UserMove != null)
                        {
                            var userMove = request.UserMove;

                            // Get customer information
                            var memberCustomer = await _unitOfWork.CustomerRepository.Get(
                                filter: c => c.UserId == userMove.UserId,
                                includeProperties: "User");

                            var memberInfo = memberCustomer.FirstOrDefault();

                            if (memberInfo != null && memberInfo.User != null)
                            {
                                // Check if this customer is a tenant in the room
                                var roomStayCustomer = await _unitOfWork.RoomStayCustomerRepository.Get(
                                    filter: rsc => rsc.RoomStayId == roomStay.RoomStayId &&
                                                  rsc.CustomerId == memberInfo.CustomerId &&
                                                  rsc.Type.Equals(CustomerTypeEnums.Tenant.ToString()) &&
                                                  rsc.Status.Equals(StatusEnums.Active.ToString()));

                                if (roomStayCustomer != null && roomStayCustomer.Any())
                                {
                                    var tenantInfo = new TenantInfo
                                    {
                                        UserId = userMove.UserId,
                                        CustomerId = memberInfo.CustomerId,
                                        Email = memberInfo.User.Email,
                                        FullName = memberInfo.User.FullName,
                                        Dob = memberInfo.User.Dob,
                                        Avatar = memberInfo.User.Avatar,
                                        RoomId = roomStay.RoomId ?? string.Empty,
                                        RoomNumber = roomStay.Room?.RoomNumber,
                                        //Title = roomStay.Room?.Title,
                                    };

                                    DesignatedInfo designatedInfo = new DesignatedInfo();
                                    if (request.UserDeposite != null)
                                    {
                                        var userDeposite = request.UserDeposite;

                                        // Get customer information for the designated person
                                        var designatedCustomer = await _unitOfWork.CustomerRepository.Get(
                                            filter: c => c.UserId == userDeposite.UserId,
                                            includeProperties: "User");

                                        var designatedMember = designatedCustomer.FirstOrDefault();

                                        if (designatedMember != null && designatedMember.User != null)
                                        {
                                            designatedInfo = new DesignatedInfo
                                            {
                                                DesignatedId = userDeposite.UserId,
                                                CustomerId = designatedMember.CustomerId,
                                                Email = designatedMember.User.Email,
                                                FullName = designatedMember.User.FullName,
                                                Avatar = designatedMember.User.Avatar
                                            };
                                        }
                                    }

                                    // Add this tenant's leave request to the result
                                    var tenantMoveOutRes = new TenantMoveOutRes
                                    {
                                        Cmoid = request.Cmoid,
                                        UserMoveId = request.UserMoveId,
                                        UserDepositeId = request.UserDepositeId,
                                        RoomStayId = request.RoomStayId,
                                        DateRequest = request.DateRequest,
                                        Status = request.Status,
                                        TenantInfo = tenantInfo,
                                        DesignatedInfo = designatedInfo
                                    };

                                    result.LeaveRoomRequestList.Add(tenantMoveOutRes);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public async Task<DetailTenantLeaveRoomRes> GetDetailTenantLeaveRoomRequest(string cmoId)
        {
            if (string.IsNullOrEmpty(cmoId))
            {
                throw new ArgumentException("Customer Move Out ID cannot be null or empty.");
            }

            var moveOutRequest = (await _unitOfWork.CustomerMoveOutRepository.Get(
                filter: cmo => cmo.Cmoid == cmoId,
                includeProperties: "UserMove,UserDeposite")).FirstOrDefault();

            if (moveOutRequest == null)
            {
                throw new KeyNotFoundException($"Move out request with ID {cmoId} not found.");
            }

            var result = new DetailTenantLeaveRoomRes();
            var detailMoveOutRes = new DetailTenantMoveOutRes
            {
                Cmoid = moveOutRequest.Cmoid,
                UserMoveId = moveOutRequest.UserMoveId,
                UserDepositeId = moveOutRequest.UserDepositeId,
                RoomStayId = moveOutRequest.RoomStayId,
                DateRequest = moveOutRequest.DateRequest,
                Status = moveOutRequest.Status
            };

            var roomStay = (await _unitOfWork.RoomStayRepository.Get(
                filter: rs => rs.RoomStayId == moveOutRequest.RoomStayId,
                includeProperties: "Room")).FirstOrDefault();

            if (roomStay == null)
            {
                throw new KeyNotFoundException($"Room stay with ID {moveOutRequest.RoomStayId} not found.");
            }

            if (moveOutRequest.UserMove != null)
            {
                var userMove = moveOutRequest.UserMove;

                var memberCustomer = await _unitOfWork.CustomerRepository.Get(
                    filter: c => c.UserId == userMove.UserId,
                    includeProperties: "User");

                var memberInfo = memberCustomer.FirstOrDefault();

                if (memberInfo != null && memberInfo.User != null)
                {
                    detailMoveOutRes.DetailTenantInfo = new DetailTenantInfo
                    {
                        UserId = userMove.UserId,
                        CustomerId = memberInfo.CustomerId,
                        Email = memberInfo.User.Email,
                        FullName = memberInfo.User.FullName,
                        Dob = memberInfo.User.Dob,
                        PhoneNumber = memberInfo.User.PhoneNumber,
                        Gender = memberInfo.User.Gender,
                        Avatar = memberInfo.User.Avatar,
                        RoomId = roomStay.RoomId ?? string.Empty,
                        RoomNumber = roomStay.Room?.RoomNumber,
                        //Title = roomStay.Room?.Title,
                        //Description = roomStay.Room?.Description,
                        Status = roomStay.Room?.Status,
                        Location = roomStay.Room?.Location
                    };
                }
            }

            if (moveOutRequest.UserDeposite != null)
            {
                var userDeposite = moveOutRequest.UserDeposite;

                var designatedCustomer = await _unitOfWork.CustomerRepository.Get(
                    filter: c => c.UserId == userDeposite.UserId,
                    includeProperties: "User");

                var designatedMember = designatedCustomer.FirstOrDefault();

                if (designatedMember != null && designatedMember.User != null)
                {
                    detailMoveOutRes.DetailDesignatedInfo = new DetailDesignatedInfo
                    {
                        DesignatedId = userDeposite.UserId,
                        CustomerId = designatedMember.CustomerId,
                        Email = designatedMember.User.Email,
                        FullName = designatedMember.User.FullName,
                        Dob = designatedMember.User.Dob,
                        PhoneNumber = designatedMember.User.PhoneNumber,
                        Gender = designatedMember.User.Gender,
                        Avatar = designatedMember.User.Avatar
                    };
                }
            }

            result.DetailTenantMoveOutRes = detailMoveOutRes;
            return result;
        }

        public async Task<bool> AcceptTenantLeaveRoomRequest(string token, string requestId)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId,
                    includeProperties: "User")).FirstOrDefault();

            if (landlord == null)
            {
                throw new UnauthorizedAccessException("Landlord not found.");
            }

            var customerMoveOut = await _unitOfWork.CustomerMoveOutRepository.GetByIDAsync(requestId);
            if (customerMoveOut == null)
            {
                throw new KeyNotFoundException("Room leaving request not found.");
            }

            if (customerMoveOut.Status == 1)
            {
                throw new InvalidOperationException("This request has already been processed.");
            }

            var roomStay = await _unitOfWork.RoomStayRepository.GetByIDAsync(customerMoveOut.RoomStayId);
            if (roomStay == null)
            {
                throw new KeyNotFoundException("Room stay information not found.");
            }

            if (roomStay.LandlordId != landlord.LandlordId)
            {
                throw new UnauthorizedAccessException("You are not authorized to process this request.");
            }

            var userMoveId = customerMoveOut.UserMoveId;
            var cusMove = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userMoveId,
                    includeProperties: "User")).FirstOrDefault();

            if (cusMove == null)
            {
                throw new KeyNotFoundException("Tenant information not found.");
            }

            var room = await _unitOfWork.RoomRepository.GetByIDAsync(roomStay.RoomId);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            var roomStayCustomerMove = (await _unitOfWork.RoomStayCustomerRepository.Get(filter: rsc =>
                    rsc.Customer.CustomerId == cusMove.CustomerId
                    && rsc.RoomStayId == roomStay.RoomStayId
                    && rsc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();

            if (roomStayCustomerMove == null)
            {
                throw new KeyNotFoundException("Customer not in this room or already leave room.");
            }

            roomStayCustomerMove.Status = StatusEnums.Inactive.ToString();
            roomStayCustomerMove.UpdatedAt = DateTime.Now;
            await _unitOfWork.RoomStayCustomerRepository.Update(roomStayCustomerMove);
            await _unitOfWork.SaveAsync();


            if (!string.IsNullOrEmpty(customerMoveOut.UserDepositeId))
            {
                var customerContract = (await _unitOfWork.CustomerContractRepository.Get(filter: cc =>
                    cc.TenantId == cusMove.CustomerId &&
                    cc.RentalRoomId == room.RoomId &&
                    cc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
                if (customerContract == null)
                {
                    throw new KeyNotFoundException("Customer contract not found.");
                }

                var designatedCustomer = (await _unitOfWork.CustomerRepository.Get(
                    filter: c => c.UserId == customerMoveOut.UserDepositeId,
                    includeProperties: "User")).FirstOrDefault();

                if (designatedCustomer != null)
                {
                    // Check if designated person is already in the room
                    var designatedRoomStayCustomer = (await _unitOfWork.RoomStayCustomerRepository.Get(
                        filter: rsc => rsc.CustomerId == designatedCustomer.CustomerId
                                && rsc.RoomStayId == roomStay.RoomStayId
                                && rsc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();

                    if (designatedRoomStayCustomer != null)
                    {
                        // If the designated person was not a Tenant before, update their role to Tenant
                        if (!designatedRoomStayCustomer.Type.Equals(CustomerTypeEnums.Tenant.ToString()))
                        {
                            designatedRoomStayCustomer.Type = CustomerTypeEnums.Tenant.ToString();
                            designatedRoomStayCustomer.UpdatedAt = DateTime.Now;
                            await _unitOfWork.RoomStayCustomerRepository.Update(designatedRoomStayCustomer);
                            await _unitOfWork.SaveAsync();

                            //Update Tenant ID for customer contract
                            customerContract.TenantId = designatedCustomer.CustomerId;
                            customerContract.UpdatedDate = DateTime.Now;
                            await _unitOfWork.CustomerContractRepository.Update(customerContract);
                            await _unitOfWork.SaveAsync();
                        }
                    }
                }
            }

            // Check if there are any remaining active tenants in the room
            var remainingTenants = await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rsc => rsc.RoomStayId == roomStay.RoomStayId
                        && rsc.Status.Equals(StatusEnums.Active.ToString())
                        && rsc.Type.Equals(CustomerTypeEnums.Tenant.ToString()));

            // If no tenants remain, update room to Available
            if (remainingTenants == null || !remainingTenants.Any())
            {
                room.Status = StatusEnums.Available.ToString();
                await _unitOfWork.RoomRepository.Update(room);

                roomStay.Status = StatusEnums.Inactive.ToString();
                await _unitOfWork.RoomStayRepository.Update(roomStay);

                var customerContract = (await _unitOfWork.CustomerContractRepository.Get(filter: cc =>
                    cc.TenantId == cusMove.CustomerId &&
                    cc.RentalRoomId == room.RoomId &&
                    cc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
                if (customerContract == null)
                {
                    throw new KeyNotFoundException("Customer contract not found.");
                }

                customerContract.Status = StatusEnums.Inactive.ToString();
                customerContract.UpdatedDate = DateTime.Now;
                await _unitOfWork.CustomerContractRepository.Update(customerContract);
                await _unitOfWork.SaveAsync();
            }



            customerMoveOut.Status = 1;
            await _unitOfWork.CustomerMoveOutRepository.Update(customerMoveOut);



            // Get designated deposit person information for email
            string designatedPersonName = "";
            if (!string.IsNullOrEmpty(customerMoveOut.UserDepositeId))
            {
                var designatedUser = await _unitOfWork.UserRepository.GetByIDAsync(customerMoveOut.UserDepositeId);
                if (designatedUser != null)
                {
                    designatedPersonName = designatedUser.FullName;
                }
            }

            var mailLeaveRoomAcceptedForTenant = EmailTemplate.LeaveRoomAcceptedNotificationForTenant(
                cusMove.User.FullName,
                landlord.User.FullName,
                room.RoomNumber,
                room.Location,
                DateTime.Now.ToString("dd/MM/yyyy")
            );

            var cusMoveMail = cusMove.User.Email;
            await _emailService.SendEmail(
                cusMoveMail,
                "Thông báo yêu cầu rời phòng của bạn đã được chấp nhận",
                mailLeaveRoomAcceptedForTenant
            );

            var mailTenantLeaveRoomAcceptedForLandlord = EmailTemplate.TenantLeaveRoomAcceptedNotificationForLandlord(
                landlord.User.FullName,
                cusMove.User.FullName,
                room.RoomNumber,
                room.Location,
                DateTime.Now.ToString("dd/MM/yyyy"),
                designatedPersonName
            );

            var landlordMail = landlord.User.Email;
            await _emailService.SendEmail(
                landlordMail,
                "Xác nhận chấp nhận yêu cầu rời phòng của thành viên",
                mailTenantLeaveRoomAcceptedForLandlord);

            // Send notification to designated person if exists
            if (!string.IsNullOrEmpty(customerMoveOut.UserDepositeId))
            {
                var designatedUser = await _unitOfWork.UserRepository.GetByIDAsync(customerMoveOut.UserDepositeId);
                if (designatedUser != null)
                {
                    var mailDesignatedPerson = EmailTemplate.LeaveRoomDesignatedPersonNotification(
                        designatedUser.FullName,
                        cusMove.User.FullName,
                        room.RoomNumber,
                        room.Location,
                        DateTime.Now.ToString("dd/MM/yyyy")
                    );

                    await _emailService.SendEmail(
                        designatedUser.Email,
                        "Thông báo: Bạn được chỉ định xử lý tiền đặt cọc và trở thành người thuê chính",
                        mailDesignatedPerson
                    );
                }
            }

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> KickTenantbyLanlord(string token, KickTenantReq request)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: c => c.UserId == userId,
                    includeProperties: "User")).FirstOrDefault();
            if (landlord == null)
            {
                throw new UnauthorizedAccessException("Landlord not found.");
            }


            var memberKickedId = request.customerId;
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

            var roomStayCustomer = (await _unitOfWork.RoomStayCustomerRepository.Get(
                            filter: rsc => rsc.LandlordId == landlord.LandlordId && 
                            rsc.CustomerId == memberKickedId &&
                            rsc.Status.Equals(StatusEnums.Active.ToString())
                            )).FirstOrDefault();
            if (roomStayCustomer == null)
            {
                throw new KeyNotFoundException("The selected member is not in your room or they have left.");
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


            List<string> evidenceImageUrls = new List<string>();
            if (request.evidenceImages != null && request.evidenceImages.Any())
            {
                try
                {
                    evidenceImageUrls = await _cloudinaryStorageService.UploadImageAsync(request.evidenceImages);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to upload evidence images: {ex.Message}");
                }
            }

            roomStayCustomer.Status = StatusEnums.Inactive.ToString();
            await _unitOfWork.RoomStayCustomerRepository.Update(roomStayCustomer);
            await _unitOfWork.SaveAsync();

            // Check if there are any remaining active tenants in the room
            var remainingTenants = await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rsc => rsc.RoomStayId == roomStay.RoomStayId
                        && rsc.Status.Equals(StatusEnums.Active.ToString())
                        && rsc.Type.Equals(CustomerTypeEnums.Tenant.ToString()));

            // If no tenants remain, update room to Available
            if (remainingTenants == null || !remainingTenants.Any())
            {
                room.Status = StatusEnums.Available.ToString();
                await _unitOfWork.RoomRepository.Update(room);

                roomStay.Status = StatusEnums.Inactive.ToString();
                await _unitOfWork.RoomStayRepository.Update(roomStay);

                var customerContract = (await _unitOfWork.CustomerContractRepository.Get(filter: cc =>
                        cc.TenantId == memberKicked.CustomerId &&
                        cc.RentalRoomId == room.RoomId &&
                        cc.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
                if (customerContract == null)
                {
                    throw new KeyNotFoundException("Customer contract not found.");
                }

                customerContract.Status = StatusEnums.Inactive.ToString();
                customerContract.UpdatedDate = DateTime.Now;
                await _unitOfWork.CustomerContractRepository.Update(customerContract);
                await _unitOfWork.SaveAsync();
            }

            var kickReason = ReasonEnums.KickRoommateReason.ToString();
            if (!string.IsNullOrEmpty(request.reason)) { kickReason = request.reason; }

            if (roomStayCustomer.Customer?.User != null)
            {
                var requesterEmail = roomStayCustomer.Customer.User.Email;
                var tenantName = roomStayCustomer.Customer.User.FullName ?? "";
                var landlordName = landlord.User.FullName ?? "Chủ trọ";
                var roomNumber = room.RoomNumber ?? "";
                var roomAddress = room.Location ?? "Không có địa chỉ";
                var date = DateTime.Now.ToString("dd/MM/yyyy");

                var kickTenantMail = EmailTemplate.TenantKickedByLandlord(
                    tenantName,
                    landlordName,
                    roomNumber,
                    roomAddress,
                    kickReason,
                    date,
                    evidenceImageUrls);

                await _emailService.SendEmail(
                    requesterEmail,
                    "Thông báo chấm dứt thuê phòng",
                    kickTenantMail);
            }

            return true;
        }
    }
}
