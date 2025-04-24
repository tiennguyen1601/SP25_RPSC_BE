using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class ExtendContractRequest
{
    public string RequestId { get; set; } = null!;

    public string? Status { get; set; }

    public int? MonthWantToRent { get; set; }

    public string? MessageCustomer { get; set; }

    public string? MessageLandlord { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ContractId { get; set; }

    public string? LandlordId { get; set; }

    public virtual CustomerContract? Contract { get; set; }

    public virtual Landlord? Landlord { get; set; }
}
