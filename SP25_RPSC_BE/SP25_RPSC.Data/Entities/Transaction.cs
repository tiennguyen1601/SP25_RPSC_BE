using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Transaction
{
    public string TransactionId { get; set; } = null!;

    public string TransactionNumber { get; set; } = null!;

    public string TransactionInfo { get; set; } = null!;

    public string Type { get; set; } = null!;

    public double Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public string Status { get; set; } = null!;

    public string? LcontractId { get; set; }

    public virtual LandlordContract? Lcontract { get; set; }
}
