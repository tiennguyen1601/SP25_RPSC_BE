using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomStay
{
    public string RoomStayId { get; set; } = null!;

    public string? RoomId { get; set; }

    public string? LandlordId { get; set; }

    public string? Status { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CustomerMoveOut> CustomerMoveOuts { get; set; } = new List<CustomerMoveOut>();

    public virtual Landlord? Landlord { get; set; }

    public virtual Room? Room { get; set; }

    public virtual ICollection<RoomStayCustomer> RoomStayCustomers { get; set; } = new List<RoomStayCustomer>();
}
