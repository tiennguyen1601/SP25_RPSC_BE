using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Address
{
    public string AddressId { get; set; } = null!;

    public string? City { get; set; }

    public string? District { get; set; }

    public string? Street { get; set; }

    public string? HouseNumber { get; set; }

    public double? Long { get; set; }

    public double? Lat { get; set; }

    public string? RoomId { get; set; }

    public virtual Room? Room { get; set; }
}
