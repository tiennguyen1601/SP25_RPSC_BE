using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? StartDateContract { get; set; }

    public DateTime? EndDateContract { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? LcontractId { get; set; }

    public virtual LandlordContract? Lcontract { get; set; }
}
