using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class LandlordContract
{
    public string LcontractId { get; set; } = null!;

    public DateTime? SignedDate { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? LcontractUrl { get; set; }

    public string? LandlordSignatureUrl { get; set; }

    public string? PackageId { get; set; }

    public string? LandlordId { get; set; }

    public virtual Landlord? Landlord { get; set; }

    public virtual ServicePackage? Package { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
