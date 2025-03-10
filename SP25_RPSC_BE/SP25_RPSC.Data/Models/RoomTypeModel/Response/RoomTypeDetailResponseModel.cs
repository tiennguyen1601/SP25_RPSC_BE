using SP25_RPSC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomTypeModel.Response
{
    public class RoomTypeDetailResponseModel
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public decimal? Deposite { get; set; }
        public decimal? Square { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string LandlordName { get; set; }
        public string Address { get; set; }
        public List<string> RoomImageUrls { get; set; }
        public List<decimal?> RoomPrices { get; set; }
        public List<string> RoomServiceNames { get; set; }
        public List<decimal?> RoomServicePrices { get; set; }
    }
}
