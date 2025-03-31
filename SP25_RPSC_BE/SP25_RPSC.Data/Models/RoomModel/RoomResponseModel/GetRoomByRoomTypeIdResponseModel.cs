using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
        public class GetRoomByRoomTypeIdResponseModel
        {
            public List<ListRoomResByRoomTypeId> Rooms { get; set; }
            public int TotalRooms { get; set; }
        }

        public class ListRoomResByRoomTypeId
        {
            public string RoomId { get; set; }
            public string RoomNumber { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
            public string Location { get; set; }
            public string RoomTypeId { get; set; }
            public string RoomTypeName { get; set; }
            public decimal Price { get; set; }
            public List<string> RoomImages { get; set; }
            public List<RoomAmentyDto> Amenties { get; set; }
        }
            public class RoomAmentyDto
            {
                public string RoomAmentyId { get; set; }
                public string RoomId { get; set; }
                public string Name { get; set; }  
                public decimal? Compensation { get; set; } 
            }


}
