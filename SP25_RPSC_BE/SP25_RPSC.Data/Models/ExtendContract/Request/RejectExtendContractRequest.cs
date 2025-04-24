using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.ExtendContract.Request
{
    public class RejectExtendContractRequest
    {
        public string RequestId { get; set; } = null!;
        public string MessageLandlord { get; set; } = null!;
    }
}
