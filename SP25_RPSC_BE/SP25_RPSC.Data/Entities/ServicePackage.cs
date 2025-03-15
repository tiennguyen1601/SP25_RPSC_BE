using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class ServicePackage
{
    public string PackageId { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string HighLight { get; set; } = null!;

    public string? Size { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<LandlordContract> LandlordContracts { get; set; } = new List<LandlordContract>();

    public virtual ICollection<ServiceDetail> ServiceDetails { get; set; } = new List<ServiceDetail>();
}
