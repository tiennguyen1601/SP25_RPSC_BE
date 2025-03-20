﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.DecodeTokenModel;
using SP25_RPSC.Data.Models.RoomModel.RoomResponseModel;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;

namespace SP25_RPSC.Services.Service.RoomServices
{
    public class RoomServices : IRoomServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDecodeTokenHandler _decodeTokenHandler;

        public RoomServices(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _decodeTokenHandler = decodeTokenHandler;
        }


        public async Task<GetRequiresRoomRentalByLandlordResponseModel> GetRequiresRoomRentalByLandlordId(
    string token, string searchQuery, int pageIndex, int pageSize)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();

            Expression<Func<Room, bool>> searchFilter = room =>
                room.RoomType != null &&
                room.RoomType.LandlordId == landlord.LandlordId &&
                room.RoomRentRequests.Any(r => r.Status == "Pending") &&
                (string.IsNullOrEmpty(searchQuery) ||
                 room.RoomNumber.Contains(searchQuery) ||
                 room.Title.Contains(searchQuery));

            var rooms = await _unitOfWork.RoomRepository.Get(
                        includeProperties: "RoomType,RoomRentRequests.CustomerRentRoomDetailRequests,RoomImages,RoomPrices",
                        filter: searchFilter,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );


            var totalRooms = await _unitOfWork.RoomRepository.CountAsync(searchFilter);

            if (rooms == null || !rooms.Any())
            {
                return new GetRequiresRoomRentalByLandlordResponseModel
                {
                    Rooms = new List<ListRoomRes>(),
                    TotalRooms = 0
                };
            }

            var roomResponses = rooms.Select(room => new ListRoomRes
            {
                RoomId = room.RoomId,
                RoomTypeId = room.RoomTypeId,
                RoomRentRequestsId = room.RoomRentRequests.FirstOrDefault()?.RoomRentRequestsId ?? "",
                RoomNumber = room.RoomNumber,
                Title = room.Title,
                Description = room.Description,
                Status = room.Status,
                Location = room.Location,
                RoomTypeName = room.RoomType?.RoomTypeName ?? "N/A",
                TotalRentRequests = room.RoomRentRequests
                        .Sum(r => r.CustomerRentRoomDetailRequests.Count(c => c.Status == "Pending")),

                RoomImages = room.RoomImages.Select(img => img.ImageUrl).ToList(),

                Price = room.RoomPrices
                    .OrderByDescending(p => p.ApplicableDate) 
                    .Select(p => p.Price)
                    .FirstOrDefault()

            }).ToList();

            return new GetRequiresRoomRentalByLandlordResponseModel
            {
                Rooms = roomResponses,
                TotalRooms = totalRooms
            };
        }





    }
}
