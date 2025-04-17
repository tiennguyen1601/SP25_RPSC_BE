using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.ExtendContract.Request
{
    public class CreateRequestExtendContract
    {
        public string MessageLandlord { get; set; }
        public string MessageCustomer { get; set; }
        public int MonthWantToRent { get; set; }
        public string Status { get; set; } = "Pending";
        public string LandlordId { get; set; }
        public string ContractId { get; set; }
    }
}
