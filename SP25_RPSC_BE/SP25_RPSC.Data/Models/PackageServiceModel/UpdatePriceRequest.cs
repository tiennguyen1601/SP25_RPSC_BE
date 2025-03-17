using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PackageServiceModel
{
    public class UpdatePriceRequest
    {
        public decimal NewPrice { get; set; }
        public string NewName { get; set; }
        public string NewDuration { get; set; }
        public string NewDescription { get; set; }
    }

}
