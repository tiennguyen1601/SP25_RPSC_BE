using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomAmenty
{
    public string RoomAmentyId { get; set; } = null!;

    public string? Name { get; set; }

    public decimal? Compensation { get; set; }

    public string? LandlordId { get; set; }

    public virtual Landlord? Landlord { get; set; }

    public virtual ICollection<RoomAmentiesList> RoomAmentiesLists { get; set; } = new List<RoomAmentiesList>();
}
