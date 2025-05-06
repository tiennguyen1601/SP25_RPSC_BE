using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.RoomTypeModel.Request;
using SP25_RPSC.Data.Models.RoomTypeModel.RequestModel;
using SP25_RPSC.Data.Models.RoomTypeModel.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.JWTService;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
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
        private readonly IDecodeTokenHandler _decodeTokenHandler;

        public RoomTypeService(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _decodeTokenHandler = decodeTokenHandler;
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
            return await _unitOfWork.RoomTypeRepository.UpdateRoomTypeStatus(roomTypeId, "Available");
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

        public async Task<bool> CreateRoomType(RoomTypeCreateRequestModel model, string token)
        {
            var phoneNum = _decodeTokenHandler.decode(token).phoneNumber;
            var existingUser = await _unitOfWork.LandlordRepository.GetLandlordByPhoneNumber(phoneNum);
            if (string.IsNullOrEmpty(model.RoomTypeName))
            {
                throw new ApiException(HttpStatusCode.BadRequest, "RoomTypeName is required");
            }

            if (model.Deposite < 0 || model.Square < 0 || model.Area < 0)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Invalid numeric values");
            }

            var address = new Address
            {
                AddressId = Guid.NewGuid().ToString(),
                Long = model.location.Long,
                Lat = model.location.Lat,
                HouseNumber = model.location.HouseNumber,
                District = model.location.District,
                Street = model.location.Street,
                City = model.location.City
            };

            var roomTypeServices = model.ListRoomServices.Select(s =>
            {
                var serviceId = Guid.NewGuid().ToString();
                return new RoomService
                {
                    RoomServiceId = serviceId,
                    RoomServiceName = s.RoomServiceName,
                    Description = s.Description,
                    Status = StatusEnums.Active.ToString(),
                    CreatedAt = DateTime.Now,
                    RoomServicePrices = new List<RoomServicePrice>
                    {
                    new RoomServicePrice
                    {
                    RoomServicePriceId = Guid.NewGuid().ToString(),
                    Price = s.Price.Price,
                    ApplicableDate = DateTime.Now,
                    RoomServiceId = serviceId
                    }
                    }   
                };
            }).ToList();

            var roomType = new RoomType
            {
                RoomTypeName = model.RoomTypeName,
                Deposite = model.Deposite,
                Area = model.Area,
                Square = model.Square,
                Description = model.Description,
                MaxOccupancy = model.MaxOccupancy,
                Status = StatusEnums.Available.ToString(),
                RoomServices = roomTypeServices,
                UpdatedAt = DateTime.Now,
                Landlord = existingUser,
                Address = address,
            };

            await _unitOfWork.RoomTypeRepository.Add(roomType);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<GetRoomTypeResponseModel> GetRoomTypeByLandlordId(string searchQuery, int pageIndex, int pageSize, string token, string status)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();

            if (landlord == null)
            {
                throw new UnauthorizedAccessException("Landlord not found");
            }

            Expression<Func<RoomType, bool>> searchFilter = r =>
                (string.IsNullOrEmpty(searchQuery) ||
                 r.RoomTypeName.Contains(searchQuery) ||
                 r.Description.Contains(searchQuery)) &&
                r.LandlordId == landlord.LandlordId &&
                (string.IsNullOrEmpty(status) || r.Status == status);


            var roomTypes = await _unitOfWork.RoomTypeRepository.Get(
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: c => c.OrderByDescending(c => c.CreatedAt)
            );

            var totalRoomTypes = await _unitOfWork.RoomTypeRepository.CountAsync(searchFilter);

            if (!roomTypes.Any())
            {
                return new GetRoomTypeResponseModel { RoomTypes = new List<ListRoomTypeRes>(), TotalRoomTypes = 0 };
            }

            var roomTypeResponses = _mapper.Map<List<ListRoomTypeRes>>(roomTypes.ToList());

            return new GetRoomTypeResponseModel
            {
                RoomTypes = roomTypeResponses,
                TotalRoomTypes = totalRoomTypes
            };
        }


        public async Task<GetRoomTypeDetailResponseModel> GetRoomTypeDetailByRoomTypeId(string roomTypeId)
        {
            var roomType = (await _unitOfWork.RoomTypeRepository.Get(
                includeProperties: "Address,RoomServices,RoomServices.RoomServicePrices",
                filter: rt => rt.RoomTypeId == roomTypeId
            )).FirstOrDefault();

            if (roomType == null)
            {
                throw new KeyNotFoundException("RoomType not found.");
            }

            roomType.RoomServices = roomType.RoomServices
                .Where(rs => rs.Status.Equals(StatusEnums.Active.ToString()))
                .ToList();

            var roomTypeResponse = _mapper.Map<GetRoomTypeDetailResponseModel>(roomType);

            foreach (var roomService in roomType.RoomServices)
            {
                var latestPrice = roomService.RoomServicePrices
                    .OrderByDescending(rsp => rsp.ApplicableDate)
                    .FirstOrDefault()?.Price;

                var roomServiceDto = roomTypeResponse.RoomServices
                    .FirstOrDefault(rs => rs.RoomServiceId == roomService.RoomServiceId);

                if (roomServiceDto != null)
                {
                    roomServiceDto.Price = latestPrice;
                }
            }

            return roomTypeResponse;
        }

        public async Task<bool> UpdateRoomTypeAsync(RoomTypeUpdateRequestModel model, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var existingRoomType = await _unitOfWork.RoomTypeRepository.GetRoomTypeDetail(model.RoomTypeId);
            if (existingRoomType == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"Room type with ID {model.RoomTypeId} not found");
            }

            if (existingRoomType.Landlord.UserId != userId)
            {
                throw new ApiException(HttpStatusCode.Forbidden, "You don't have permission to update this room type");
            }

            existingRoomType.RoomTypeName = model.RoomTypeName;
            existingRoomType.Deposite = model.Deposite;
            existingRoomType.Area = model.Area;
            existingRoomType.Square = model.Square;
            existingRoomType.Description = model.Description;
            existingRoomType.MaxOccupancy = model.MaxOccupancy;
            existingRoomType.UpdatedAt = DateTime.Now;

            if (existingRoomType.Address == null)
            {
                existingRoomType.Address = new Address
                {
                    AddressId = Guid.NewGuid().ToString()
                };
            }

            existingRoomType.Address.Long = model.Location.Long;
            existingRoomType.Address.Lat = model.Location.Lat;
            existingRoomType.Address.Street = model.Location.Street;
            existingRoomType.Address.District = model.Location.District;
            existingRoomType.Address.City = model.Location.City;
            existingRoomType.Address.HouseNumber = model.Location.HouseNumber;

            await _unitOfWork.RoomTypeRepository.Update(existingRoomType);

            if (model.ListRoomServices != null && model.ListRoomServices.Any())
            {
                await UpdateRoomServices(existingRoomType, model.ListRoomServices);
            }

            await _unitOfWork.SaveAsync();

            return true;
        }


        private async Task UpdateRoomServices(RoomType roomType, ICollection<RoomServiceRequestUpdate> serviceModels)
        {
            // Lấy danh sách RoomService hiện tại của RoomType
            var existingServices = await _unitOfWork.RoomServiceRepository.GetServicesByRoomTypeId(roomType.RoomTypeId);
            var existingServiceIds = existingServices.Select(s => s.RoomServiceId).ToList();

            // Tách các service được yêu cầu update
            var servicesToUpdate = serviceModels
                .Where(sm => !string.IsNullOrEmpty(sm.RoomServiceId) && existingServiceIds.Contains(sm.RoomServiceId))
                .ToList();

            // Tìm các service cần deactivate
            var serviceIdsToKeep = servicesToUpdate.Select(s => s.RoomServiceId).ToList();
            var servicesToDeactivate = existingServices
                .Where(es => !serviceIdsToKeep.Contains(es.RoomServiceId))
                .ToList();

            // Deactivate những service không còn nằm trong danh sách update
            foreach (var service in servicesToDeactivate)
            {
                service.Status = StatusEnums.Inactive.ToString();
                service.UpdatedAt = DateTime.Now;
                await _unitOfWork.RoomServiceRepository.Update(service);
            }

            // Cập nhật các RoomService đã tồn tại
            foreach (var serviceModel in servicesToUpdate)
            {
                var existingService = existingServices.First(s => s.RoomServiceId == serviceModel.RoomServiceId);

                existingService.RoomServiceName = serviceModel.RoomServiceName;
                existingService.Description = serviceModel.Description;
                existingService.UpdatedAt = DateTime.Now;
                existingService.Status = StatusEnums.Active.ToString();

                // Thêm giá mới vào list RoomServicePrices
                var newPrice = new RoomServicePrice
                {
                    RoomServicePriceId = Guid.NewGuid().ToString(),
                    Price = serviceModel.Price.Price,
                    ApplicableDate = DateTime.Now,
                    RoomServiceId = existingService.RoomServiceId
                };

                await _unitOfWork.RoomServicePriceRepository.Add(newPrice);

                await _unitOfWork.RoomServiceRepository.Update(existingService);
            }

            // Thêm mới các RoomService chưa có trong database
            var newServices = serviceModels
                .Where(sm => string.IsNullOrEmpty(sm.RoomServiceId))
                .Select(sm =>
                {
                    var newId = Guid.NewGuid().ToString();
                    return new RoomService
                    {
                        RoomServiceId = newId,
                        RoomTypeId = roomType.RoomTypeId,
                        RoomServiceName = sm.RoomServiceName,
                        Description = sm.Description,
                        Status = StatusEnums.Active.ToString(),
                        CreatedAt = DateTime.Now,
                        RoomServicePrices = new List<RoomServicePrice>
                        {
                    new RoomServicePrice
                    {
                        RoomServicePriceId = Guid.NewGuid().ToString(),
                        Price = sm.Price.Price,
                        ApplicableDate = DateTime.UtcNow,
                        RoomServiceId = newId
                    }
                        }
                    };
                })
                .ToList();

            foreach (var newService in newServices)
            {
                await _unitOfWork.RoomServiceRepository.Add(newService);
            }
        }
    }
}
