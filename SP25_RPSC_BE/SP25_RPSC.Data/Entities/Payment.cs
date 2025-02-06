using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? TransactionId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? ContractId { get; set; }

    public string? ExtendContractId { get; set; }

    public virtual CustomerContract? Contract { get; set; }

    public virtual ExtendContract? ExtendContract { get; set; }
}
