using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Landlord
{
    public string LandlordId { get; set; } = null!;

    public string? CompanyName { get; set; }

    public int? NumberRoom { get; set; }

    public string? LicenseNumber { get; set; }

    public string? BankName { get; set; }

    public string? BankNumber { get; set; }

    public string? Template { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UserId { get; set; }

    public virtual ICollection<BusinessImage> BusinessImages { get; set; } = new List<BusinessImage>();

    public virtual ICollection<LandlordContract> LandlordContracts { get; set; } = new List<LandlordContract>();

    public virtual ICollection<RoomAmenty> RoomAmenties { get; set; } = new List<RoomAmenty>();

    public virtual ICollection<RoomStayCustomer> RoomStayCustomers { get; set; } = new List<RoomStayCustomer>();

    public virtual ICollection<RoomStay> RoomStays { get; set; } = new List<RoomStay>();

    public virtual ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();

    public virtual User? User { get; set; }
}
