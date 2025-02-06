using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class ServicePackage
{
    public string PackageId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? Duration { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<LandlordContract> LandlordContracts { get; set; } = new List<LandlordContract>();

    public virtual ICollection<PricePackage> PricePackages { get; set; } = new List<PricePackage>();
}
