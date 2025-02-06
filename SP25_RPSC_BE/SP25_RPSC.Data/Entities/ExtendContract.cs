using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class ExtendContract
{
    public string ExtendContractId { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ContractId { get; set; }

    public virtual CustomerContract? Contract { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
