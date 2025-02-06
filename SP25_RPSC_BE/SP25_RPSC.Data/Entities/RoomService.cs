using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomService
{
    public string RoomServiceId { get; set; } = null!;

    public string? RoomServiceName { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? RoomTypeId { get; set; }

    public virtual ICollection<RoomServicePrice> RoomServicePrices { get; set; } = new List<RoomServicePrice>();

    public virtual RoomType? RoomType { get; set; }
}
