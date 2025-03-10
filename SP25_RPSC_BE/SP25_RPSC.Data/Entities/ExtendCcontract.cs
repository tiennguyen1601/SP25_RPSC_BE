using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class ExtendCcontract
{
    public string ExtendCcontractId { get; set; } = null!;

    public DateTime StartDateContract { get; set; }

    public DateTime EndDateContract { get; set; }

    public int? ExtendCount { get; set; }

    public string? ContractId { get; set; }

    public virtual CustomerContract? Contract { get; set; }
}
