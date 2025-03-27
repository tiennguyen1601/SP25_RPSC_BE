﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Models.RoomModel.RequestModel;
using SP25_RPSC.Data.Models.RoomModel.RoomResponseModel;

namespace SP25_RPSC.Services.Service.RoomServices
{
    public interface IRoomServices
    {
        Task<GetRequiresRoomRentalByLandlordResponseModel> GetRequiresRoomRentalByLandlordId(string token, string searchQuery, int pageIndex, int pageSize);
        Task<RoomCountResponseModel> GetRoomCountsByLandlordId(string token);

        Task<bool> CreateRoom(RoomCreateRequestModel model);
    }
}
