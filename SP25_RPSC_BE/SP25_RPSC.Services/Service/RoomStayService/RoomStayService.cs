using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using MailKit.Search;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.RoomStay;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;

namespace SP25_RPSC.Services.Service.RoomStayService
{
    public class RoomStayService : IRoomStayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDecodeTokenHandler _decodeTokenHandler;

        public RoomStayService(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _decodeTokenHandler = decodeTokenHandler;
        }

        public async Task<GetAllRoomStaysResponseModel> GetRoomStaysByLandlordId(
    string token, string searchQuery, int pageIndex, int pageSize, string status)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();

            if (landlord == null)
            {
                throw new UnauthorizedAccessException("Landlord not found");
            }

            var landlordId = landlord.LandlordId;

            Expression<Func<RoomStay, bool>> searchFilter = rs =>
                rs.LandlordId == landlordId &&
                (string.IsNullOrEmpty(searchQuery) || rs.Room.RoomNumber.Contains(searchQuery)) &&
                (string.IsNullOrEmpty(status) || rs.Status == status);

            var roomStays = await _unitOfWork.RoomStayRepository.Get(
                includeProperties: "Room,Room.RoomImages",
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalRoomStays = await _unitOfWork.RoomStayRepository.CountAsync(searchFilter);

            var roomStayResponses = _mapper.Map<List<RoomStayDto>>(roomStays);

            return new GetAllRoomStaysResponseModel
            {
                RoomStays = roomStayResponses,
                TotalRoomStays = totalRoomStays
            };
        }

        public async Task<GetRoomStayCustomersResponseModel> GetRoomStaysCustomerByRoomStayId(string roomStayId)
        {
            var roomStayCustomers = (await _unitOfWork.RoomStayCustomerRepository
                    .Get(filter: rsc => rsc.RoomStayId == roomStayId, includeProperties: "Customer.User")
                    ).ToList(); 


            if (!roomStayCustomers?.Any() ?? true)
            {
                throw new KeyNotFoundException("No customers found for this room stay.");
            }

            var roomStay = (await _unitOfWork.RoomStayRepository.Get(
                        includeProperties: "Room,Room.RoomImages,Room.RoomPrices,Room.RoomType,Room.RoomType.RoomServices,Room.RoomType.RoomServices.RoomServicePrices,Room.RoomAmentiesLists,Room.RoomAmentiesLists.RoomAmenty",
                        filter: rs => rs.RoomStayId == roomStayId
                    )).FirstOrDefault();






            if (roomStay == null)
            {
                throw new KeyNotFoundException("RoomStay not found.");
            }

            var customerResponses = _mapper.Map<List<RoomStayCustomerDto>>(roomStayCustomers);
            var roomStayResponse = _mapper.Map<RoomStayDetailsDto>(roomStay);

            return new GetRoomStayCustomersResponseModel
            {
                RoomStay = roomStayResponse,
                RoomStayCustomers = customerResponses,
                TotalCustomers = customerResponses.Count
            };
        }

    }

}
