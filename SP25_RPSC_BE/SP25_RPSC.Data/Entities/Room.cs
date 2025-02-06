using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Room
{
    public string RoomId { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public decimal? Deposite { get; set; }

    public string? Status { get; set; }

    public string? Location { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? LandlordId { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<CustomerContract> CustomerContracts { get; set; } = new List<CustomerContract>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Landlord? Landlord { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<RoomStay> RoomStays { get; set; } = new List<RoomStay>();

    public virtual ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
}
