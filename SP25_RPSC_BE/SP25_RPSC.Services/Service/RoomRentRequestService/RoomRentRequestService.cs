using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SP25_RPSC.Data.Entities;
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


        public RoomRentRequestService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IDecodeTokenHandler decodeTokenHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _decodeTokenHandler = decodeTokenHandler;
        }
        public async Task<List<CustomerRequestRes>> GetCustomersByRoomRentRequestsId(string roomRentRequestsId)
        {
            var request = (await _unitOfWork.RoomRentRequestRepository.Get(
                filter: r => r.RoomRentRequestsId == roomRentRequestsId,
                includeProperties: "CustomerRentRoomDetailRequests.Customer.User"
            )).FirstOrDefault();

            if (request == null || request.CustomerRentRoomDetailRequests == null || !request.CustomerRentRoomDetailRequests.Any())
            {
                return new List<CustomerRequestRes>();
            }

            var customers = request.CustomerRentRoomDetailRequests.Select(c => new CustomerRequestRes
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

                MonthWantRent = c.MonthWantRent ?? 6
            }).ToList();

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

            room.Status = "Renting";
            room.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.RoomRepository.Update(room);

            DateTime today = DateTime.UtcNow;
            DateTime startDate = new DateTime(today.Year, today.Month, 1).AddMonths(1);
            int monthWantRent = selectedCustomerRequest.MonthWantRent ?? 6;
            DateTime endDate = startDate.AddMonths(monthWantRent);

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
                    var successEmail = EmailTemplate.BookingSuccess(fullName, roomAddress, email, startDate.ToString("yyyy-MM-dd"));
                    await _emailService.SendEmail(email, "Xác nhận thuê phòng thành công", successEmail);
                }
                else
                {
                    customerRequest.Status = "Rejected";
                    var failureEmail = EmailTemplate.BookingFailure(fullName, roomAddress, email, "Phòng đã được thuê bởi khách khác.");
                    await _emailService.SendEmail(email, "Thông báo từ chối thuê phòng", failureEmail);
                }
            }

            request.Status = "Active";

            var newRoomStay = new RoomStay
            {
                RoomStayId = Guid.NewGuid().ToString(),
                RoomId = room.RoomId,
                LandlordId = landlord.LandlordId,
                StartDate = startDate,
                EndDate = endDate,
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
                CustomerId = selectedCustomerId,
                LandlordId = landlord.LandlordId,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.RoomStayCustomerRepository.Add(newRoomStayCustomer);

            await _unitOfWork.SaveAsync();

            return true;
        }



    }
}
