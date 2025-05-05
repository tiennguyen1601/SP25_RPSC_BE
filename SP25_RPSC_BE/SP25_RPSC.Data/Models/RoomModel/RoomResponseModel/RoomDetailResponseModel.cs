using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
    public class RoomDetailResponseModel : RoomResponseModel
    {
        public List<RoomServiceResponseModel> RoomServices { get; set; }
        public LandlordResponseModel Landlord { get; set; }
        public string? PostRoomId { get; set; }
    }

    public class RoomServiceResponseModel
    {
        public string RoomServiceId { get; set; }
        public string RoomServiceName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<RoomServicePriceResponseModel> Prices { get; set; }
    }

    public class RoomServicePriceResponseModel
    {
        public decimal? Price { get; set; }
        public DateTime? ApplicableDate { get; set; }
    }
}
