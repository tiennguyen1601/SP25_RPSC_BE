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
        public string PackageId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int? Duration { get; set; }

        public string? Description { get; set; }
        public string? serviceStatus { get; set; }


        public List<ListDetailService> ListDetails { get; set; }
     
        public class ListDetailService
        {
        public string ServiceDetailId { get; set; }
        public string Type { get; set; }
        public string? LimitPost { get; set; }
        public string Status { get; set; }
        public string PriceId { get; set; }
        public DateTime? ApplicableDate { get; set; }
        public decimal? Price { get; set; }
        }

        

}
}
