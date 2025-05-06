using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
    public class RoomResponseModel
    {
        public string RoomId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public DateTime? AvailableDateToRent { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public RoomTypeResModel RoomType { get; set; }
        public LandlordResponseModel Landlord { get; set; }
        public List<RoomPriceResponseModel> RoomPrices { get; set; }
        public List<string> RoomImages { get; set; }
        public List<RoomAmentyResponseModel> RoomAmenities { get; set; }

        public string PackageLabel { get; set; }
        public int? PackagePriorityTime { get; set; }
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }

    }


    public class RoomTypeResModel
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public int? Area { get; set; }
        public int? MaxOccupancy { get; set; }
        public string Description { get; set; }
        public AddressDTO Address { get; set; }
    }

    public class LandlordResponseModel
    {
        public string LandlordId { get; set; }
        public string LandlordName { get; set; }
        public string CompanyName { get; set; }
        public string UserId { get; set; }
    }

    public class RoomPriceResponseModel
    {
        public decimal? Price { get; set; }
        public DateTime? ApplicableDate { get; set; }
    }

    public class RoomAmentyResponseModel
    {
        public string AmenityId { get; set; }
        public string Name { get; set; }
    }

    public class AddressDTO
    {
        public string AddressId { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public double? Long { get; set; }
        public double? Lat { get; set; }
    }
}
