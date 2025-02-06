using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomServicePrice
{
    public string RoomServicePriceId { get; set; } = null!;

    public DateTime? ApplicableDate { get; set; }

    public decimal? Price { get; set; }

    public string? RoomServiceId { get; set; }

    public virtual RoomService? RoomService { get; set; }
}
