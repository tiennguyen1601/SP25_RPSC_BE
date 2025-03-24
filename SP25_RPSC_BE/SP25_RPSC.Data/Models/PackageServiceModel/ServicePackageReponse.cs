using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PackageServiceModel
{
    public class ServicePackageReponse
    {
        public string PackageId { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string HighLightTime { get; set; } = null!;

        public int? MaxPost { get; set; }
        public string Label { get; set; } = null!;

        public string? Status { get; set; }
    }
}
