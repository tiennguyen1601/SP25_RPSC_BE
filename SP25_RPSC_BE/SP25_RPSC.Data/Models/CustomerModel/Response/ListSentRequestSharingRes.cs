using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerModel.Response
{
    public class ListSentRequestSharingRes
    {
        public ListSentRequestSharingRes()
        {
            SentRequestSharingList = new List<SentRequestSharingInfo>();
        }
        public List<SentRequestSharingInfo> SentRequestSharingList { get; set; }
        public int TotalSentRequests { get; set; }
    }

    public class SentRequestSharingInfo
    {
        public string RequestId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public PostInfoForRequest PostInfo { get; set; }
    }

    public class PostInfoForRequest
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string Location { get; set; }
        public string PostOwnerName { get; set; }
        public string PostOwnerAvatar { get; set; }
        public string PostOwnerPhone { get; set; }
        public string PostOwnerEmail { get; set; }
    }
}
