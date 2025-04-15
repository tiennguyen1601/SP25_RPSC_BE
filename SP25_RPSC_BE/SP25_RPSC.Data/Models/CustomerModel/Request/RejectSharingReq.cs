using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerModel.Request
{
    public class RejectSharingReq
    {
        public string requestId { get; set; }
        public string reason { get; set; }
    }
}
