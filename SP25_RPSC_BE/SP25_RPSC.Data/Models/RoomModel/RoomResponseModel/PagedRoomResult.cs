using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Entities;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
    public class PagedRoomResult
    {
        public int TotalActivePosts { get; set; }
        public int TotalRooms { get; set; }
        public List<Room> Rooms { get; set; }
    }

}
