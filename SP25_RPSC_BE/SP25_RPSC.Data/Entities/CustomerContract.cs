using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class CustomerContract
{
    public string ContractId { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Term { get; set; }

    public string? TenantId { get; set; }

    public string? RentalRoomId { get; set; }

    public virtual ICollection<ExtendContract> ExtendContracts { get; set; } = new List<ExtendContract>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Room? RentalRoom { get; set; }

    public virtual Customer? Tenant { get; set; }
}
