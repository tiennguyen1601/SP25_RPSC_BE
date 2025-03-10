using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class BusinessImage
{
    public string BusinessImageId { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public string? Status { get; set; }

    public string? LandlordId { get; set; }

    public virtual Landlord? Landlord { get; set; }
}
