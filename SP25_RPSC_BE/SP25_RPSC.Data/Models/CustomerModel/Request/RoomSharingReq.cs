using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerModel.Request
{
    public class RoomSharingReq
    {
        public string PostId { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }
}
