using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PayOSModel
{
    public class PayOSReqModel
    {
        public int OrderId { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string PackageName { get; set; }
        public string RedirectUrl { get; set; }
        public string CancleUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
