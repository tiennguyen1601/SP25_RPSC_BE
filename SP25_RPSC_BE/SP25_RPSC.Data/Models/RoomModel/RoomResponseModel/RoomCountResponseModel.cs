using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
    public class RoomCountResponseModel
    {
        public int TotalRooms { get; set; }
        public int TotalRoomsActive { get; set; }
        public int TotalRoomsRenting { get; set; }
        public int TotalCustomersRenting { get; set; }
        public int TotalRequests { get; set; }

    }

}
