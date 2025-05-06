using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
    public class FeedbackResponseModel
    {
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Rating { get; set; }
        public string? ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public string? RentalRoomId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }

}
