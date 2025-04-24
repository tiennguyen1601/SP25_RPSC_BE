using System.Linq.Expressions;
using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.RoomStay;
using SP25_RPSC.Data.Models.RoomStayModel;
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
                    .Get(filter: rsc => rsc.RoomStayId == roomStayId && rsc.Status == "Active", includeProperties: "Customer.User")
                    ).ToList();

            if (!roomStayCustomers?.Any() ?? true)
            {
                throw new KeyNotFoundException("No customers found for this room stay.");
            }

            var roomStay = (await _unitOfWork.RoomStayRepository.Get(
                        includeProperties: "Room,Room.RoomImages,Room.RoomPrices,Room.RoomType,Room.RoomType.RoomServices,Room.RoomType.RoomServices.RoomServicePrices,Room.RoomAmentiesLists,Room.RoomAmentiesLists.RoomAmenty",
                        filter: rs => rs.RoomStayId == roomStayId && rs.Status == "Active"
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
                throw new UnauthorizedAccessException("Invalid or expired token.");
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
                    TotalRoomer = 0
                };
            }

            if (roomStayCustomer.Status == "Inactive")
            {
                return null;
            }

            var roomStayId = roomStayCustomer.RoomStayId;
            var roomStay = (await _unitOfWork.RoomStayRepository.Get(
                        includeProperties: "Room",
                        filter: rs => rs.RoomStayId == roomStayId
                    )).FirstOrDefault();

            var allRoommates = (await _unitOfWork.RoomStayCustomerRepository.Get(
                       includeProperties: "Customer,Customer.User",
                       filter: rsc => rsc.RoomStayId == roomStayId && rsc.Status == "Active"
                   )).ToList();

            var roommateInfoList = new List<RoommateInfo>();
            foreach (var roommate in allRoommates)
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
                        UserId = roommate.Customer.UserId,
                        IsCurrentUser = roommate.CustomerId == customerId
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
                TotalRoomer = roommateInfoList.Count
            };

            return response;
        }

        public async Task<GetRoomStayByCustomerIdResponseModel> GetRoomStayByCustomerId(string token)
        {
            if (token == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
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
                filter: rs => rs.CustomerId == customerId && rs.Status == "Active"
            )).FirstOrDefault();

            if (roomStayCustomer == null || roomStayCustomer.RoomStay == null)
            {
                throw new KeyNotFoundException("RoomStay not found for this customer.");
            }

            var roomStayId = roomStayCustomer.RoomStayId;

            var roomStay = (await _unitOfWork.RoomStayRepository.Get(
                includeProperties: "Room,Room.RoomImages,Room.RoomPrices,Room.RoomType,Room.RoomType.RoomServices,Room.RoomType.RoomServices.RoomServicePrices,Room.RoomAmentiesLists,Room.RoomAmentiesLists.RoomAmenty,Landlord.User",
                filter: rs => rs.RoomStayId == roomStayId
            )).FirstOrDefault();

            if (roomStay == null)
            {
                throw new KeyNotFoundException("RoomStay not found.");
            }

            decimal? latestPrice = GetLatestPrice(roomStay.Room.RoomPrices);

            var tenantCustomer = (await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rs => rs.RoomStayId == roomStayId && rs.Type == "Tenant" && rs.Status == "Active"
            )).FirstOrDefault();

            var contract = (await _unitOfWork.CustomerContractRepository.Get(
                filter: c => c.TenantId == tenantCustomer.CustomerId && c.RentalRoomId == roomStay.RoomId
            )).FirstOrDefault();

            var landlordName = roomStay.Landlord?.User?.FullName;
            var landlordAva = roomStay.Landlord?.User?.Avatar;
            var landlordId = roomStay.Landlord?.LandlordId;


            var roomStayResponse = _mapper.Map<RoomStayDetailsResponseModel>(roomStay);
            roomStayResponse.Room.Price = latestPrice;
            roomStayResponse.LandlordName = landlordName;
            roomStayResponse.LandlordAvatar = landlordAva;
            roomStayResponse.LandlordId = landlordId;

            roomStayResponse.RoomStayCustomerType = roomStayCustomer.Type;

            //var activeCustomers = roomStay.RoomStayCustomers.Get;
            var activeCustomers = (await _unitOfWork.RoomStayCustomerRepository.Get(
     rsc => rsc.RoomStayId == roomStayId && rsc.Status == "Active"
 )).ToList();  

            var numberOfGuests = activeCustomers.Count;



            var maxOccupancy = roomStay.Room?.RoomType.MaxOccupancy ?? 0;

            roomStayResponse.statusOfMaxRoom = numberOfGuests < maxOccupancy ? "NotEnough" : "Enough";


            var contractDto = _mapper.Map<CustomerContractDto>(contract);

            return new GetRoomStayByCustomerIdResponseModel
            {
                RoomStay = roomStayResponse,
                CustomerContract = contractDto
            };
        }

    }

}
