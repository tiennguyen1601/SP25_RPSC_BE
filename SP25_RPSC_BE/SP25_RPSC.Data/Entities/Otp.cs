using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Otp
{
    public string Id { get; set; } = null!;

    public string? Code { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsUsed { get; set; }

    public string? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
