using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class ServiceDetail
{
    public string ServiceDetailId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Duration { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? PackageId { get; set; }

    public virtual ServicePackage? Package { get; set; }

    public virtual ICollection<PricePackage> PricePackages { get; set; } = new List<PricePackage>();
}
