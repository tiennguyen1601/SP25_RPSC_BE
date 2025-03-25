using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
