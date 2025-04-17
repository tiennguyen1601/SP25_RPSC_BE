using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerModel.Request
{
    public class KickRoommateReq
    {
        public string customerId { get; set; }
        public string reason { get; set; } = "";
        public List<IFormFile> evidenceImages { get; set; }
    }
}
