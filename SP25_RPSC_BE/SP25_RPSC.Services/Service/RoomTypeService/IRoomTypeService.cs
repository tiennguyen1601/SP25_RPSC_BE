﻿using SP25_RPSC.Data.Models.RoomTypeModel.Request;
using SP25_RPSC.Data.Models.RoomTypeModel.RequestModel;
using SP25_RPSC.Data.Models.RoomTypeModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.RoomTypeService
{
    public interface IRoomTypeService
    {
        Task<List<RoomTypeResponseModel>> GetAllRoomTypesPending(int pageIndex, int pageSize);
        Task<RoomTypeDetailResponseModel> GetRoomTypeDetail(string roomTypeId);
        Task<bool> ApproveRoomType(string roomTypeId);
        Task<bool> DenyRoomType(string roomTypeId);
        Task<bool> CreateRoomType(RoomTypeCreateRequestModel model, string token);
        Task<GetRoomTypeResponseModel> GetRoomTypeByLandlordId(string searchQuery, int pageIndex, int pageSize, string token,string status);
        Task<GetRoomTypeDetailResponseModel> GetRoomTypeDetailByRoomTypeId(string roomTypeId);
        Task<bool> UpdateRoomTypeAsync(RoomTypeUpdateRequestModel model, string token);
    }

}
