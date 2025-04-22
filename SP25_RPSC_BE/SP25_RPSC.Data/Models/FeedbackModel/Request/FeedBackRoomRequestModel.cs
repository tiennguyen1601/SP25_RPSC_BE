using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.FeedbackModel.Request
{
    public class FeedBackRoomRequestModel
    {
        public string Description { get; set; }

        public int Rating { get; set; }

        public string RentalRoomId { get; set; }

        public required List<IFormFile> Images { get; set; }
    }
}
