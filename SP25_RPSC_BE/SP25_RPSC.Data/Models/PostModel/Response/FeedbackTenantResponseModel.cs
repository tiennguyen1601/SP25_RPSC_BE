using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PostModel.Response
{
    public class FeedbackTenantResponseModel
    {
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Rating { get; set; }
        public string? ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public string? RevieweeId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }
}
