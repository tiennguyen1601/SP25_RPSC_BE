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
    string token, string searchQuery, int pageIndex, int pageSize)
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
                (string.IsNullOrEmpty(searchQuery) || rs.Room.RoomNumber.Contains(searchQuery));

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

            decimal? latestPrice = GetLatestPrice(roomStay.Room.RoomPrices);

            var customerResponses = _mapper.Map<List<RoomStayCustomerDto>>(roomStayCustomers);
            var roomStayResponse = _mapper.Map<RoomStayDetailsDto>(roomStay);

            roomStayResponse.Room.Price = latestPrice;

            return new GetRoomStayCustomersResponseModel
            {
                RoomStay = roomStayResponse,
                RoomStayCustomers = customerResponses,
                TotalCustomers = customerResponses.Count
            };
        }

        private decimal? GetLatestPrice(IEnumerable<RoomPrice> roomPrices)
        {
            if (roomPrices == null || !roomPrices.Any())
                return null;

            return roomPrices.OrderByDescending(p => p.ApplicableDate).FirstOrDefault()?.Price;
        }


        public async Task<ListRoommateRes> GetListRoommate(string token)
        {
            if (token == null)
            {
                throw new UnauthorizedAccessException("Missing token!");
            }
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var customer = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userId)).FirstOrDefault();
            if (customer == null)
            {
                throw new UnauthorizedAccessException("Customer not found");
            }

            var customerId = customer.CustomerId;
            var roomStayCustomer = (await _unitOfWork.RoomStayCustomerRepository.Get(
                        includeProperties: "RoomStay",
                        filter: rs => rs.CustomerId == customerId
                    )).FirstOrDefault();

            if (roomStayCustomer == null || roomStayCustomer.RoomStay == null)
            {
                return new ListRoommateRes
                {
                    RoomStay = null,
                    RoommateList = new List<RoommateInfo>(),
                    TotalRoomate = 0
                };
            }

            var roomStayId = roomStayCustomer.RoomStayId;
            var roomStay = (await _unitOfWork.RoomStayRepository.Get(
                        includeProperties: "Room",
                        filter: rs => rs.RoomStayId == roomStayId
                    )).FirstOrDefault();


            var roommates = (await _unitOfWork.RoomStayCustomerRepository.Get(
                       includeProperties: "Customer,Customer.User",
                       filter: rsc => rsc.RoomStayId == roomStayId && rsc.CustomerId != customerId && rsc.Status == "Active"
                   )).ToList();

            var roommateInfoList = new List<RoommateInfo>();
            foreach (var roommate in roommates)
            {
                if (roommate.Customer?.User != null)
                {
                    roommateInfoList.Add(new RoommateInfo
                    {
                        CustomerId = roommate.CustomerId,
                        RoomerType = roommate.Type,
                        CustomerType = roommate.Customer.CustomerType,
                        Email = roommate.Customer.User.Email,
                        FullName = roommate.Customer.User.FullName,
                        Dob = roommate.Customer.User.Dob,
                        Address = roommate.Customer.User.Address,
                        PhoneNumber = roommate.Customer.User.PhoneNumber,
                        Gender = roommate.Customer.User.Gender,
                        Avatar = roommate.Customer.User.Avatar,
                        Preferences = roommate.Customer.Preferences,
                        LifeStyle = roommate.Customer.LifeStyle,
                        BudgetRange = roommate.Customer.BudgetRange,
                        PreferredLocation = roommate.Customer.PreferredLocation,
                        Requirement = roommate.Customer.Requirement,
                        UserId = roommate.Customer.UserId
                    });
                }
            }

            var response = new ListRoommateRes
            {
                RoomStay = roomStay != null ? new RoomStayInfo
                {
                    RoomStayId = roomStay.RoomStayId,
                    RoomId = roomStay.RoomId,
                    LandlordId = roomStay.LandlordId,
                    StartDate = roomStay.StartDate,
                    EndDate = roomStay.EndDate,
                    Status = roomStay.Status
                } : null,
                RoommateList = roommateInfoList,
                TotalRoomer = 1 + roommates.Count 
            };

            return response;

        }

    }

}
