using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomPrice
{
    public string RoomPriceId { get; set; } = null!;

    public DateTime? ApplicableDate { get; set; }

    public decimal? Price { get; set; }

    public string? RoomTypeId { get; set; }

    public virtual RoomType? RoomType { get; set; }
}
