using SP25_RPSC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PackageModel
{
    public class PackageCreateRequestModel
    {
        [JsonIgnore]
        public Guid PackageId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        public string Description { get; set; }

        public int Duration { get; set; }

        public ICollection<PackageCreateDetailReqestModel> PackageDetails { get; set; }
    }

    public class PackageCreateDetailReqestModel
    {
        [JsonIgnore]
        public Guid ServiceDetailId { get; set; } = Guid.NewGuid();
        [JsonIgnore]
        public Guid PackageId { get; set; }
        public string Type { get; set; }
        public int LimitPost { get; set; }
        public PricePackageRequestModel PricePackageModel { get; set; }
    }

    public class PricePackageRequestModel
    {
        [JsonIgnore]
        public Guid PriceId { get; set; } = Guid.NewGuid();

        public decimal Price { get; set; }
        
        public DateTime? ApplicableDate { get; set; }

    }
}
