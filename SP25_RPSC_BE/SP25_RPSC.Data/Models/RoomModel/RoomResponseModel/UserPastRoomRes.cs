using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
    public class UserPastRoomRes
    {
        public string RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string RoomTypeName { get; set; }
        public string Address { get; set; }
        public List<string> Images { get; set; }
    }

}
