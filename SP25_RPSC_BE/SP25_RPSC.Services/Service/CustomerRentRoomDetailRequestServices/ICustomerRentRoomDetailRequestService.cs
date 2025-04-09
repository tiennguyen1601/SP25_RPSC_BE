using SP25_RPSC.Data.Models.RoomRentModel;
using SP25_RPSC.Data.Models.RoomRentRequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.CustomerRentRoomDetailRequestServices
{
    public interface ICustomerRentRoomDetailRequestService
    {
        Task<RoomRentResponseModel> CreateRentRequest(RoomRentRequestCreateModel model, string token);
        //Task<RoomRentResponseModel> LandlordGetRentRequestByRoomId(string roomId, string token);

        //Task<RoomRentResponseModel> CustomerGetRentRequestByUserId(string token);

        Task<IEnumerable<RoomRentResponseModel>> GetRoomRentRequestsByRoomId(string roomId, string token);
    }
}
