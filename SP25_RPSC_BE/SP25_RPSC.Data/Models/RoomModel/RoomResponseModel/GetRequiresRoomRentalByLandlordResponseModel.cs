using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
    public class GetRequiresRoomRentalByLandlordResponseModel
    {
        public List<ListRoomRes> Rooms { get; set; } = new List<ListRoomRes>();  
        public int TotalRooms { get; set; }  
    }

    public class ListRoomRes
    {
        public string RoomId { get; set; }
        public string RoomTypeId { get; set; }
        public string RoomRentRequestsId { get; set; }
        public string RoomNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string RoomTypeName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Square { get; set; }
        public int? Area { get; set; }
        public List<string> RoomImages { get; set; } = new List<string>();
        public int TotalRentRequests { get; set; }
    }
}
