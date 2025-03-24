using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Models.RoomStay;

namespace SP25_RPSC.Services.Service.RoomStayService
{
    public interface IRoomStayService
    {
        Task<GetAllRoomStaysResponseModel> GetRoomStaysByLandlordId(string token, string searchQuery, int pageIndex, int pageSize);
        Task<GetRoomStayCustomersResponseModel> GetRoomStaysCustomerByRoomStayId(string roomStayId);
    }
}
