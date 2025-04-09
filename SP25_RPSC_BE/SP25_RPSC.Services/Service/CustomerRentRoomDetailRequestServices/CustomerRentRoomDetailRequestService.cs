using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.CustomerRentRoomDetail.CustomerRentRoomDetailResponse;
using SP25_RPSC.Data.Models.RoomRentModel;
using SP25_RPSC.Data.Models.RoomRentRequestModel;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
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

        public Task<RoomRentResponseModel> GetRentRequestById(Guid id, string token)
        {
            throw new NotImplementedException();
        }
    }
}
