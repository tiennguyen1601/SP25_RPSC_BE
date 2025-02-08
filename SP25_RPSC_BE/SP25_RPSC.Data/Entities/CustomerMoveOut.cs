using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class CustomerMoveOut
{
    public string Cmoid { get; set; } = null!;

    public string? UserMoveId { get; set; }

    public string? UserDepositeId { get; set; }

    public string? RoomStayId { get; set; }

    public DateTime? DateRequest { get; set; }

    public int? Status { get; set; }

    public virtual RoomStay? RoomStay { get; set; }

    public virtual User? UserDeposite { get; set; }

    public virtual User? UserMove { get; set; }
}
