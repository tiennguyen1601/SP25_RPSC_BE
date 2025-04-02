using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.FeedbackModel.Request
{
    public class FeedBackCustomerRequestModel
    {
        public string Description { get; set; }

        public string Type { get; set; }

        public int Rating { get; set; }

        public string Status { get; set; }

        public string RevieweeId { get; set; }

        public required List<IFormFile> Images { get; set; }
    }
}
