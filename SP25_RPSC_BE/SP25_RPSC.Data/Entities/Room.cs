using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Room
{
    public string RoomId { get; set; } = null!;

    public string? RoomNumber { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? Location { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? LandlordId { get; set; }

    public string? RoomTypeId { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<CustomerContract> CustomerContracts { get; set; } = new List<CustomerContract>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Landlord? Landlord { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<RoomAmentiesList> RoomAmentiesLists { get; set; } = new List<RoomAmentiesList>();

    public virtual ICollection<RoomStay> RoomStays { get; set; } = new List<RoomStay>();

    public virtual RoomType? RoomType { get; set; }
}
