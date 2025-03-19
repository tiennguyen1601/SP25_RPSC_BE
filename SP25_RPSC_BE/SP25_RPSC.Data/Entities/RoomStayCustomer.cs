using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomStayCustomer
{
    public string RoomStayCustomerId { get; set; } = null!;

    public string? Type { get; set; }

    public string? Status { get; set; }

    public string? RoomStayId { get; set; }

    public string? CustomerId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? LandlordId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Landlord? Landlord { get; set; }

    public virtual RoomStay? RoomStay { get; set; }
}
