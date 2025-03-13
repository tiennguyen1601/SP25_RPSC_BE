using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PayOSModel
{
    public class PayOSPackageRequestDTO
    {
        public string PackageName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int TotalPrice { get; set; }
    }
}
