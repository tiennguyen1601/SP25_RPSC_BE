using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class PostRoom
{
    public string PostRoomId { get; set; } = null!;

    public DateTime? DateUpPost { get; set; }

    public int? DateExist { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? RoomId { get; set; }

    public virtual Room? Room { get; set; }
}
