using SP25_RPSC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.FeedbackModel.Response
{
    public class MyFeedbackRes
    {
        public int TotalFeedback { get; set; }
        public List<MyFeedbackInfo> MyFeedbacks { get; set; } = new List<MyFeedbackInfo>(); 
    }

    public class MyFeedbackInfo
    {
        public string FeedbackId { get; set; } = null!;
        public string? Description { get; set; }
        public int? Rating { get; set; }
        public string? Type { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Status { get; set; }
        public RevieweeInfo RevieweeInfo { get; set; }
        public RentalRoomInfo RentalRoomInfo { get; set; }
        public virtual ICollection<ImageRf> ImageRves { get; set; } = new List<ImageRf>();
    }

    public class RevieweeInfo {
        public string RevieweeId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
    }

    public class RentalRoomInfo
    {
        public string RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string RoomTypeName { get; set; }
        public string Address { get; set; }
        public List<string> Images { get; set; }
    }
}
