using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Customer
{
    public string CustomerId { get; set; } = null!;

    public string? Preferences { get; set; }

    public string? LifeStyle { get; set; }

    public string? BudgetRange { get; set; }

    public string? PreferredLocation { get; set; }

    public string? Requirement { get; set; }

    public string? Status { get; set; }

    public string? UserId { get; set; }

    public virtual ICollection<CustomerContract> CustomerContracts { get; set; } = new List<CustomerContract>();

    public virtual ICollection<CustomerRentRoomDetailRequest> CustomerRentRoomDetailRequests { get; set; } = new List<CustomerRentRoomDetailRequest>();

    public virtual ICollection<CustomerRequest> CustomerRequests { get; set; } = new List<CustomerRequest>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<RoomStayCustomer> RoomStayCustomers { get; set; } = new List<RoomStayCustomer>();

    public virtual User? User { get; set; }
}
