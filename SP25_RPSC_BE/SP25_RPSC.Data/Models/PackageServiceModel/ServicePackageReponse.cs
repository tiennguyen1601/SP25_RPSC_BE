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

        public string Name { get; set; } = null!;

        public int? Duration { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }
    }
}
