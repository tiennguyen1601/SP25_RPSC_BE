using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomAmenty
{
    public string RoomAmentyId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Compensation { get; set; }

    public string? RoomTypeId { get; set; }

    public virtual RoomType? RoomType { get; set; }
}
