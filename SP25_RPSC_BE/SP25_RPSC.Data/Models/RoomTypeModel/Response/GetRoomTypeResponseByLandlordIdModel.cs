using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomTypeModel.Response
{
    public class GetRoomTypeResponseModel
    {
        public List<ListRoomTypeRes> RoomTypes { get; set; }
        public int TotalRoomTypes { get; set; } 
    }
    public class ListRoomTypeRes
    {
        public string RoomTypeId { get; set; }   
        public string RoomTypeName { get; set; }
        public decimal? Deposite { get; set; }   
        public int? MaxOccupancy { get; set; }  
        public string Status { get; set; }       
        public string Description { get; set; } 
    }

}
