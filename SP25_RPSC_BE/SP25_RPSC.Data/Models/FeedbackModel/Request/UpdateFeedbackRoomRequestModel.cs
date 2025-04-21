using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.FeedbackModel.Request
{
    public class UpdateFeedbackRoomRequestModel
    {
        public string Description { get; set; }
        public int Rating { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
