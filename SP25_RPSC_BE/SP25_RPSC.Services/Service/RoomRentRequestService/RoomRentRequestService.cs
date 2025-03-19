using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SP25_RPSC.Data.Models.CustomerRentRoomDetail.CustomerRentRoomDetailResponse;
using SP25_RPSC.Data.UnitOfWorks;

namespace SP25_RPSC.Services.Service.RoomRentRequestService
{
    public class RoomRentRequestService : IRoomRentRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public RoomRentRequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

                Message = c.Message ?? "N/A" 
            }).ToList();

            return customers;
        }
    }
}
