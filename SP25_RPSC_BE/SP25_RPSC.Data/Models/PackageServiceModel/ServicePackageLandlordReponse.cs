using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Models.PackageServiceModel
{
    public class ServicePackageLandlordResponse
    {
        public string PackageId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int? Duration { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public List<ServicePriceResponse> ListServicePrice { get; set; } = new();
    }

    public class ServicePriceResponse
    {
        public string ServiceDetailId { get; set; } = null!;

        public string? Type { get; set; }

        public string? LimitPost { get; set; }

        public string PriceId { get; set; } = null!;

        public decimal? Price { get; set; }
    }
}
