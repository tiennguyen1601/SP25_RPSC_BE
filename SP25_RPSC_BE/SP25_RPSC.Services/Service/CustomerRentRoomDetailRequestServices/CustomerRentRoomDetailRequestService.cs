using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Org.BouncyCastle.Asn1.Crmf;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.CustomerRentRoomDetail.CustomerRentRoomDetailResponse;
using SP25_RPSC.Data.Models.RoomRentModel;
using SP25_RPSC.Data.Models.RoomRentRequestModel;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using System.ComponentModel;
using System.Net;

namespace SP25_RPSC.Services.Service.CustomerRentRoomDetailRequestServices
{
    public class CustomerRentRoomDetailRequestService : ICustomerRentRoomDetailRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDecodeTokenHandler _decodeTokenHandler;



        public CustomerRentRoomDetailRequestService(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _decodeTokenHandler = decodeTokenHandler;
        }

        public async Task<RoomRentResponseModel> CreateRentRequest(RoomRentRequestCreateModel model, string token)
        {
            var room = await _unitOfWork.RoomRepository.GetByIDAsync(model.RoomId);
            if (room == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"Room with ID {model.RoomId} not found");
            }

            if (room.Status != StatusEnums.Available.ToString())
            {
                throw new ApiException(HttpStatusCode.BadRequest, "This room is not available for rent");
            }

            var userId = _decodeTokenHandler.decode(token).userid;
            var customer = await _unitOfWork.CustomerRepository.GetCustomerByUserId(userId);
            if (customer == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Customer not found");
            }

            if (model.MonthWantRent < DateTime.UtcNow.Month || model.DateWantToRent < DateTime.UtcNow)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid dates. Dates must be in the future");
            }


            var roomRentRequest = new RoomRentRequest
            {
                RoomRentRequestsId = Guid.NewGuid().ToString(),
                RoomId = model.RoomId,
                Status = StatusEnums.Pending.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            var detailRequest = new CustomerRentRoomDetailRequest
            {
                CustomerRentRoomDetailRequestId = Guid.NewGuid().ToString(),
                //RoomRequestId = roomRentRequest.RoomRequestId,
                Message = model.Message,
                Status = StatusEnums.Pending.ToString(),
                MonthWantRent = model.MonthWantRent,
                DateWantToRent = model.DateWantToRent,
                CustomerId = customer.CustomerId,
                RoomRentRequests = roomRentRequest,
            };

            await _unitOfWork.RoomRentRequestRepository.Add(roomRentRequest);
            await _unitOfWork.CustomerRentRoomDetailRequestRepositories.Add(detailRequest);
            await _unitOfWork.SaveAsync();

            var response = new RoomRentResponseModel
            {
                RoomRequestId = roomRentRequest.RoomRentRequestsId,
                RoomId = roomRentRequest.RoomId,
                CustomerId = detailRequest.CustomerId,
                Status = roomRentRequest.Status,
                CreatedAt = roomRentRequest.CreatedAt,
                Message = detailRequest.Message,
                MonthWantRent = detailRequest.MonthWantRent,
                DateWantToRent = detailRequest.DateWantToRent
            };

        return response;
        }

        public Task<RoomRentResponseModel> CustomerGetRentRequestByUserId(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RoomRentResponseModel>> GetRoomRentRequestsByRoomId(string roomId, string token)
        {
       
            var user = _decodeTokenHandler.decode(token);

            var landlord = await _unitOfWork.LandlordRepository.GetLandlordByPhoneNumber(user.phoneNumber);
            var isLandlord = landlord != null;

            var customer = await _unitOfWork.CustomerRepository.GetCustomerByUserId(user.userid);
            var isCustomer = customer != null;


            var result = new List<RoomRentResponseModel>();

            if (!isLandlord && !isCustomer)
            {
                throw new ApiException(HttpStatusCode.Unauthorized, "User not found");
            }

            else if (isCustomer)
            {
                // For customers, only return their own requests for this room
                //var rentRequests = await _unitOfWork.RoomRentRequestRepository.GetByRoomIdAndCustomerId(roomId, customer.CustomerId);
                //var detailRequests = await _unitOfWork.CustomerRentRoomDetailRequestRepositories.GetByRoomRequestIds(
                //    rentRequests.Select(r => r.RoomRequestId).ToList());

                //return MapRentRequests(rentRequests, detailRequests);
                var temp1 = await _unitOfWork.CustomerRentRoomDetailRequestRepositories.GetRoomRentRequestByCustomerId(customer.CustomerId);
                foreach (var item in temp1)
                {
                    result.Add(new RoomRentResponseModel
                    {
                        RoomRequestId = item.RoomRentRequestsId,
                        RoomId = item.RoomRentRequests.RoomId,
                        CustomerId = item.CustomerId,
                        Status = item.RoomRentRequests.Status,
                        CreatedAt = item.RoomRentRequests.CreatedAt,
                        Message = item.Message,
                        MonthWantRent = item.MonthWantRent,
                        DateWantToRent = item.DateWantToRent
                    });
                }
                return result;
            }
            var room = await _unitOfWork.RoomRepository.GetByIDAsync(roomId);
            if (room == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"Room with ID {roomId} not found");
            }
            if (room.RoomType.Landlord.LandlordId != landlord.LandlordId)
            {
                throw new ApiException(HttpStatusCode.Forbidden, "You don't have permission to view requests for this room");
            }


            var temp2 = await _unitOfWork.RoomRentRequestRepository.GetRoomRentRequestByRoomId(room.RoomId);
            foreach (var item in temp2)
            {
                result.Add(new RoomRentResponseModel
                {
                    RoomRequestId = item.RoomRentRequestsId,
                    RoomId = item.RoomId,
                    CustomerId = item.CustomerRentRoomDetailRequests.FirstOrDefault()?.CustomerId,
                    Status = item.CustomerRentRoomDetailRequests.FirstOrDefault()?.Status,
                    CreatedAt = item.CreatedAt,
                    Message = item.CustomerRentRoomDetailRequests.FirstOrDefault()?.Message,
                    MonthWantRent = item.CustomerRentRoomDetailRequests?.FirstOrDefault()?.MonthWantRent,
                    DateWantToRent = item.CustomerRentRoomDetailRequests?.FirstOrDefault()?.DateWantToRent,
                });
            }

            return result;
        }
    }
}
