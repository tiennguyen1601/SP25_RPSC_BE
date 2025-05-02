using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Models.RoomModel.RoomResponseModel;

namespace SP25_RPSC.Data.Models.RoomStay
{
    public class GetAllRoomStaysResponseModel
    {
        public List<RoomStayDto> RoomStays { get; set; } = new List<RoomStayDto>();
        public int TotalRoomStays { get; set; }
    }

    public class RoomStayDto
    {
        public string RoomStayId { get; set; } = null!;
        public string? RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<RoomPriceDto> RoomPrices { get; set; } = new();
        public List<RoomAmenityDto> RoomAmenties { get; set; } = new();
    }
    public class RoomPriceDto
    {
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class RoomAmenityDto
    {
        public string? AmenityId { get; set; }
        public string? AmenityName { get; set; }
    }


}
