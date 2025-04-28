using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
    public class PostRoomDetailResponseModel
    {
        public string PostRoomId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateUpPost { get; set; }
        public int? DateExist { get; set; }
        public string Status { get; set; }
        public DateTime? AvailableDateToRent { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public RoomDetailForPostResponseModel RoomDetail { get; set; }
        public LandlordInfoResponseModel Landlord { get; set; }
        public string PackageLabel { get; set; }
        public int? PackagePriorityTime { get; set; }
    }

    public class RoomDetailForPostResponseModel
    {
        public string RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string RoomTypeName { get; set; }
        public decimal? Square { get; set; }
        public double? Area { get; set; }
        public decimal Price { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<AmenityResponseModel> Amenities { get; set; } = new List<AmenityResponseModel>();
    }

    public class AmenityResponseModel
    {
        public string AmenityId { get; set; }
        public string AmenityName { get; set; }
        public string Description { get; set; }
    }

    public class LandlordInfoResponseModel
    {
        public string LandlordId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
    }
}
