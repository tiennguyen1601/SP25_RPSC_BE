using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Chat
{
    public string ChatId { get; set; } = null!;

    public string? SenderId { get; set; }

    public string? ReceiverId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string? Status { get; set; }

    public virtual User? Receiver { get; set; }

    public virtual User? Sender { get; set; }
}
