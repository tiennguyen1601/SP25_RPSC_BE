using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class PricePackage
{
    public string PriceId { get; set; } = null!;

    public DateTime? ApplicableDate { get; set; }

    public decimal? Price { get; set; }

    public string? PackageId { get; set; }

    public virtual ServicePackage? Package { get; set; }
}
