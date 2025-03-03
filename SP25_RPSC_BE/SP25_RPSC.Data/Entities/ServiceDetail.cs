using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class ServiceDetail
{
    public string ServiceDetailId { get; set; } = null!;

    public string? Type { get; set; }

    public int? LimitPost { get; set; }

    public string? Status { get; set; }

    public string? PackageId { get; set; }

    public virtual ServicePackage? Package { get; set; }

    public virtual ICollection<PricePackage> PricePackages { get; set; } = new List<PricePackage>();
}
