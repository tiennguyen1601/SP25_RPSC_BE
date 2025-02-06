using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoommateRequest
{
    public string RequestId { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }

    public string? PostId { get; set; }

    public virtual ICollection<CustomerRequest> CustomerRequests { get; set; } = new List<CustomerRequest>();

    public virtual Post? Post { get; set; }
}
