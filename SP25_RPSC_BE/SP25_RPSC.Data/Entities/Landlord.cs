using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Landlord
{
    public string LandlordId { get; set; } = null!;

    public string? CompanyName { get; set; }

    public int? NumberRoom { get; set; }

    public string? LiscenseNumber { get; set; }

    public string? BusinessLiscense { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UserId { get; set; }

    public virtual ICollection<LandlordContract> LandlordContracts { get; set; } = new List<LandlordContract>();

    public virtual ICollection<RoomStayCustomer> RoomStayCustomers { get; set; } = new List<RoomStayCustomer>();

    public virtual ICollection<RoomStay> RoomStays { get; set; } = new List<RoomStay>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual User? User { get; set; }
}
