using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerModel.Request
{
    public class UpdateLandlordRequestModel
    {
        public string? CompanyName { get; set; }
        //public int? NumberRoom { get; set; }
        public string? LicenseNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? Template { get; set; }
        public string? Status { get; set; }
    }

}
