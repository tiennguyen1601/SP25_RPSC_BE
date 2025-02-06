using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Notification
{
    public string NotificationId { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UserId { get; set; }

    public virtual User? User { get; set; }
}
