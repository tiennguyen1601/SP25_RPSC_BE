using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PostModel.Response
{
    public class RoommatePostDetailRes
    {
        public string PostId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public PostOwnerInfo PostOwnerInfo { get; set; }
        public RoomInfo RoomInfo { get; set; }  
    }

    public class RoomInfo
    {
        public string RoomId { get; set; }
        public string LandlordName { get; set; }
        public string RoomNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string RoomTypeName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Square { get; set; }
        public int? Area { get; set; }
        public int TotalRoomer { get; set; }
        public List<string> RoomImages { get; set; } = new List<string>();
        public List<string> RoomAmenities { get; set; } = new List<string>();
        public List<RoomServiceInfo> Services { get; set; } = new List<RoomServiceInfo>();
    }

    public class RoomServiceInfo
    {
        public string ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
    }

}
