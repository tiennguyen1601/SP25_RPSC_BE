using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.FeedbackModel.Response
{
    public class FeedbackDetailResDTO
    {
        public string ReviewerName { get; set; }
        public string ReviewerPhoneNumber { get; set; }
        public string Type { get; set; }
        public string RoomNumber { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
