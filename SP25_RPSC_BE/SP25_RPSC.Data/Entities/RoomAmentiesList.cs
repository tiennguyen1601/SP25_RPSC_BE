using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomAmentiesList
{
    public string RoomAmenitiesListId { get; set; } = null!;

    public string? Description { get; set; }

    public string? RoomId { get; set; }

    public string? RoomAmentyId { get; set; }

    public string? Status { get; set; }

    public virtual Room? Room { get; set; }

    public virtual RoomAmenty? RoomAmenty { get; set; }
}
