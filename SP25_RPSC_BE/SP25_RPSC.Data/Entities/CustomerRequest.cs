using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class CustomerRequest
{
    public string CustomerRequestId { get; set; } = null!;

    public string? Message { get; set; }

    public string? Status { get; set; }

    public string? RequestId { get; set; }

    public string? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual RoommateRequest? Request { get; set; }
}
