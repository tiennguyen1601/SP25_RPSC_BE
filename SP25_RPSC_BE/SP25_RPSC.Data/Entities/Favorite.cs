using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Favorite
{
    public string FavoriteId { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public string? Status { get; set; }

    public string? RoomId { get; set; }

    public string? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Room? Room { get; set; }
}
