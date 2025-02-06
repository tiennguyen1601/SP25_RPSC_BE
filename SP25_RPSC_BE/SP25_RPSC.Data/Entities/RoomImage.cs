using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomImage
{
    public string ImageId { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? RoomTypeId { get; set; }

    public virtual RoomType? RoomType { get; set; }
}
