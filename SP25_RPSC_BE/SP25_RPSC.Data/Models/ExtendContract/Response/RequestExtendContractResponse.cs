using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.ExtendContract.Response
{
    public class RequestExtendContractResponse
    {
        public string RequestExtendContractId { get; set; }
        public string MessageLandlord { get; set; }
        public string MessageCustomer { get; set; }
        public int? MonthWantToRent { get; set; }
        public string Status { get; set; }
        public string LandlordId { get; set; }
        public string ContractId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
