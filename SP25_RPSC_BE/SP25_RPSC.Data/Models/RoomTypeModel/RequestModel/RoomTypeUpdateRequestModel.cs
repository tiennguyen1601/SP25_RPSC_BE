using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.RoomTypeModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomTypeModel.RequestModel
{
    public class RoomTypeUpdateRequestModel
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public decimal Deposite { get; set; }
        public int Area { get; set; }
        public decimal Square { get; set; }
        public string Description { get; set; }
        public int MaxOccupancy { get; set; }
        public Location Location { get; set; }
        public ICollection<RoomServiceRequestUpdate> ListRoomServices { get; set; }
    }

    public class RoomServiceRequestUpdate
    {
        public Guid? RoomServiceId { get; set; } // Optional for existing services
        public string RoomServiceName { get; set; }
        public string Description { get; set; }
        public RoomServicePriceRequest Price { get; set; }
    }
}
