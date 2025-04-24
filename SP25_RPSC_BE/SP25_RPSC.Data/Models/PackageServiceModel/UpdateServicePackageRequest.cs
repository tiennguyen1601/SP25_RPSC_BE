using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PackageServiceModel
{
    public class UpdateServicePackageRequest
    {
        public string NewType { get; set; }
        public string NewHighLightTime { get; set; }
        public int? NewPriorityTime { get; set; }
        public int? NewMaxPost { get; set; }
        public string NewLabel { get; set; }
        public string NewStatus { get; set; }
    }

}
