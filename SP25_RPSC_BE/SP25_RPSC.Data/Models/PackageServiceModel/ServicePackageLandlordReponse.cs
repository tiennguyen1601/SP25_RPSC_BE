using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Models.PackageServiceModel
{
    public class ServicePackageLandlordResponse
    {
        public string PackageId { get; set; } = null!;

        public string? Status { get; set; }


        public string Type { get; set; } = null!;

        public string HighLightTime { get; set; } = null!;


        public int? MaxPost { get; set; }
        public string Label { get; set; }


        public List<ServicePriceResponse> ListServicePrice { get; set; } = new();
    }

    public class ServicePriceResponse
    {
        public string ServiceDetailId { get; set; } = null!;
        public string PriceId { get; set; } = null!;

        public decimal? Price { get; set; }
        public DateTime? ApplicableDate { get; set; }


        public string? Name { get; set; }

        public string? Duration { get; set; }

        public string? Description { get; set; }


        public string? PackageId { get; set; }
    }
}
