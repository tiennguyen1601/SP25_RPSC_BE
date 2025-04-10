using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerRequest;
using SP25_RPSC.Data.Models.CustomerRentRoomDetail.CustomerRentRoomDetailResponse;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.EmailService;
using SP25_RPSC.Services.Service.JWTService;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Services.Utils.Email;

namespace SP25_RPSC.Services.Service.RoomRentRequestService
{
    public class RoomRentRequestService : IRoomRentRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IDecodeTokenHandler _decodeTokenHandler;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;



        public RoomRentRequestService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IDecodeTokenHandler decodeTokenHandler
            , ICloudinaryStorageService cloudinaryStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _decodeTokenHandler = decodeTokenHandler;
            _cloudinaryStorageService = cloudinaryStorageService;
        }
        public async Task<List<CustomerRequestRes>> GetCustomersByRoomRentRequestsId(string roomRentRequestsId)
        {
            var request = (await _unitOfWork.RoomRentRequestRepository.Get(
                filter: r => r.RoomRentRequestsId == roomRentRequestsId &&
                            r.CustomerRentRoomDetailRequests.Any(c => c.Status == "Pending"), 
                includeProperties: "CustomerRentRoomDetailRequests.Customer.User"
            )).FirstOrDefault();

            if (request == null || request.CustomerRentRoomDetailRequests == null || !request.CustomerRentRoomDetailRequests.Any())
            {
                return new List<CustomerRequestRes>();
            }

            var customers = request.CustomerRentRoomDetailRequests
                .Where(c => c.Status == "Pending")  
                .Select(c => new CustomerRequestRes
                {
                    CustomerId = c.CustomerId,
                    Status = c.Status,
                    Preferences = c.Customer?.Preferences ?? "N/A",
                    LifeStyle = c.Customer?.LifeStyle ?? "N/A",
                    BudgetRange = c.Customer?.BudgetRange ?? "N/A",
                    PreferredLocation = c.Customer?.PreferredLocation ?? "N/A",
                    Requirement = c.Customer?.Requirement ?? "N/A",

                    FullName = c.Customer?.User?.FullName ?? "N/A",
                    Email = c.Customer?.User?.Email ?? "N/A",
                    PhoneNumber = c.Customer?.User?.PhoneNumber ?? "N/A",
                    Avatar = c.Customer?.User?.Avatar ?? "N/A",
                    Message = c.Message ?? "",
                    DateWantToRent = c.DateWantToRent,
                    MonthWantRent = c.MonthWantRent ?? 6
                })
                .ToList();

            return customers;
        }
        public async Task<bool> AcceptCustomerAndRejectOthers(string token, string roomRentRequestsId, string selectedCustomerId)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Landlord not found");
            }

            var request = (await _unitOfWork.RoomRentRequestRepository.Get(
                filter: r => r.RoomRentRequestsId == roomRentRequestsId,
                includeProperties: "CustomerRentRoomDetailRequests.Customer,Room"
            )).FirstOrDefault();

            if (request == null || request.CustomerRentRoomDetailRequests == null || !request.CustomerRentRoomDetailRequests.Any())
            {
                throw new ApiException(HttpStatusCode.NotFound, "Room rent request not found or has no customers");
            }

            var selectedCustomerRequest = request.CustomerRentRoomDetailRequests
                .FirstOrDefault(c => c.CustomerId == selectedCustomerId);

            if (selectedCustomerRequest == null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Selected customer not found in request");
            }

            var room = request.Room;
            if (room == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Room not found");
            }

            if (room.Status == "Renting")
            {
                throw new ApiException(HttpStatusCode.Conflict, "Room is already rented");
            }

            bool roomOccupied = (await _unitOfWork.RoomStayRepository.Get(
                                 filter: rs => rs.RoomId == room.RoomId && rs.Status == "Active"
                             )).Any();

            if (roomOccupied)
            {
                throw new ApiException(HttpStatusCode.Conflict, "Room is already occupied");
            }

            DateTime today = DateTime.UtcNow;
            DateTime startDate = new DateTime(today.Year, today.Month, 1).AddMonths(1);
            int monthWantRent = selectedCustomerRequest.MonthWantRent ?? 6;
            DateTime endDate = startDate.AddMonths(monthWantRent);

            DateTime customerRequestedDate = selectedCustomerRequest.DateWantToRent ?? startDate;

            var roomAddress = room.Location ?? "Không xác định";

            foreach (var customerRequest in request.CustomerRentRoomDetailRequests)
            {
                var customer = (await _unitOfWork.CustomerRepository.Get(
                    filter: c => c.CustomerId == customerRequest.CustomerId,
                    includeProperties: "User"
                )).FirstOrDefault();

                if (customer == null || customer.User == null) continue;

                string fullName = customer.User.FullName ?? "Người dùng";
                string email = customer.User.Email ?? "";

                if (customerRequest.CustomerId == selectedCustomerId)
                {
                    customerRequest.Status = "Accepted";
                    customerRequest.DateWantToRent = customerRequestedDate;

                    var successEmail = EmailTemplate.BookingSuccess(fullName, roomAddress, email, customerRequestedDate.ToString("yyyy-MM-dd"));
                    await _emailService.SendEmail(email, "Xác nhận thuê phòng thành công", successEmail);
                }
                else
                {
                    customerRequest.Status = "Rejected";
                    var failureEmail = EmailTemplate.BookingFailure(fullName, roomAddress, email, "Phòng đã được thuê bởi khách khác.");
                    await _emailService.SendEmail(email, "Thông báo từ chối thuê phòng", failureEmail);
                }
            }

            var allRequestsForCustomer = await _unitOfWork.CustomerRentRoomDetailRequestRepositories.Get(
                filter: r => r.CustomerId == selectedCustomerId && r.Status == "Pending" && r.RoomRentRequestsId != roomRentRequestsId
            );

            foreach (var customerRequest in allRequestsForCustomer)
            {
                customerRequest.Status = "Inactive";
            }

            request.Status = "Active";

            var newContract = new CustomerContract
            {
                ContractId = Guid.NewGuid().ToString(),
                StartDate = startDate,
                EndDate = endDate,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                Term = "",
                TenantId = selectedCustomerId,
                RentalRoomId = room.RoomId
            };
            await _unitOfWork.CustomerContractRepository.Add(newContract);

            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> ConfirmContractAndCreateRoomStay(string token, ContractUploadRequest request)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var tokenModel = _decodeTokenHandler.decode(token);
                var userId = tokenModel.userid;

                var landlord = (await _unitOfWork.LandlordRepository
                                    .Get(filter: l => l.UserId == userId))
                                    .FirstOrDefault();

                if (landlord == null) throw new ApiException(HttpStatusCode.NotFound, "Landlord not found");

                var contract = (await _unitOfWork.CustomerContractRepository
                                .Get(filter: c => c.ContractId == request.ContractId, includeProperties: "Tenant,RentalRoom"))
                                .FirstOrDefault();

                if (contract == null) throw new ApiException(HttpStatusCode.NotFound, "Contract not found");

                var room = contract.RentalRoom;
                if (room == null) throw new ApiException(HttpStatusCode.NotFound, "Room not found");
                if (room.Status == "Renting") throw new ApiException(HttpStatusCode.Conflict, "Room is already rented");

                string contractPdfUrl = "";
                if (request.ContractFile != null)
                {
                    var uploadResult = await _cloudinaryStorageService.UploadImageAsync(new List<IFormFile> { request.ContractFile });
                    contractPdfUrl = uploadResult.FirstOrDefault();
                }

                room.Status = "Renting";
                room.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.RoomRepository.Update(room);

                var newRoomStay = new RoomStay
                {
                    RoomStayId = Guid.NewGuid().ToString(),
                    RoomId = room.RoomId,
                    LandlordId = landlord.LandlordId,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    Status = "Active",
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.RoomStayRepository.Add(newRoomStay);

                var newRoomStayCustomer = new RoomStayCustomer
                {
                    RoomStayCustomerId = Guid.NewGuid().ToString(),
                    Type = "Tenant",
                    Status = "Active",
                    RoomStayId = newRoomStay.RoomStayId,
                    CustomerId = contract.TenantId,
                    LandlordId = landlord.LandlordId,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.RoomStayCustomerRepository.Add(newRoomStayCustomer);

                contract.Status = "Active";
                contract.UpdatedDate = DateTime.UtcNow;
                contract.Term = contractPdfUrl;
                _unitOfWork.CustomerContractRepository.Update(contract);

                await _unitOfWork.SaveAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}

