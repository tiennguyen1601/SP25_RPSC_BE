using AutoMapper;
using Microsoft.AspNetCore.Http;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.RoomTypeModel.Request;
using SP25_RPSC.Data.Models.RoomTypeModel.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace SP25_RPSC.Services.Service.RoomTypeService
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<RoomTypeResponseModel>> GetAllRoomTypesPending(int pageIndex, int pageSize)
        {
            Expression<Func<RoomType, bool>> filter = rt => rt.Status == "Pending";

            var roomTypes = await _unitOfWork.RoomTypeRepository.Get(
                includeProperties: "Landlord",
                filter: filter,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: rt => rt.OrderByDescending(r => r.CreatedAt)
            );

            if (roomTypes == null || !roomTypes.Any())
            {
                return new List<RoomTypeResponseModel>();
            }

            var response = _mapper.Map<List<RoomTypeResponseModel>>(roomTypes);

            return response;
        }

        public async Task<RoomTypeDetailResponseModel> GetRoomTypeDetail(string roomTypeId)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetRoomTypeDetail(roomTypeId);

            if (roomType == null)
            {
                return null;
            }

            var response = _mapper.Map<RoomTypeDetailResponseModel>(roomType);

            return response;
        }

        public async Task<bool> ApproveRoomType(string roomTypeId)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetRoomTypeDetail(roomTypeId);

            if (roomType == null)
            {
                return false;
            }
            if (roomType.Status != "Pending")
            {
                return false;
            }
            return await _unitOfWork.RoomTypeRepository.UpdateRoomTypeStatus(roomTypeId, "Active");
        }

        public async Task<bool> DenyRoomType(string roomTypeId)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetRoomTypeDetail(roomTypeId);

            if (roomType == null)
            {
                return false; 
            }
            if (roomType.Status != "Pending")
            {
                return false;
            }
            return await _unitOfWork.RoomTypeRepository.UpdateRoomTypeStatus(roomTypeId, "Inactive");
        }

        public async Task<bool> CreateRoomType(RoomTypeCreateRequestModel model, string phonenum)
        {

            var existingUser = await _unitOfWork.LandlordRepository.GetLandlordByPhoneNumber(phonenum);
            if (string.IsNullOrEmpty(model.RoomTypeName))
            {
                throw new ApiException(HttpStatusCode.BadRequest, "RoomTypeName is required");
            }

            if (model.Deposite < 0 || model.Square < 0 || model.Area < 0)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid numeric values");
            }

            var roomTypeServices = _mapper.Map<List<RoomService>>(model.ListRoomServices);

            var roomType = new RoomType
            {
                RoomTypeName = model.RoomTypeName,
                Deposite = model.Deposite,
                Area = model.Area,
                Square = model.Square,
                Description = model.Description,
                MaxOccupancy = model.MaxOccupancy,
                Status = StatusEnums.Pending.ToString(),
                RoomServices = roomTypeServices,
                UpdatedAt = DateTime.UtcNow,
                Landlord = existingUser,
            };

            await _unitOfWork.RoomTypeRepository.Add(roomType);
            return true;
        }
    }
}
