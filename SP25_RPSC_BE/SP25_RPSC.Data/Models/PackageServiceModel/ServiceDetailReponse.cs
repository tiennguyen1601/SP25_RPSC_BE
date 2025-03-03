using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Entities;

namespace SP25_RPSC.Data.Models.PackageServiceModel
{
    public class ServiceDetailReponse
    {
        public string ServiceDetailId { get; set; }
        public string Type { get; set; }
        public int? LimitPost { get; set; }
        public string Status { get; set; }
        public string PackageId { get; set; }

        public decimal? Price { get; set; }

        public DateTime? ApplicableDate { get; set; }

}
}
