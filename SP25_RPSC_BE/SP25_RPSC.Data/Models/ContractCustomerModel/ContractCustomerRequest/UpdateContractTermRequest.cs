using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerRequest
{
    public class UpdateContractTermRequest
    {
        public string ContractId { get; set; } = null!;
        public string NewTermUrl { get; set; } = null!;
    }

}
