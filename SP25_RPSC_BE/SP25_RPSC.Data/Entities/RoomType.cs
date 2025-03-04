using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomType
{
    public string RoomTypeId { get; set; } = null!;

    public string? RoomTypeName { get; set; }

    public decimal? Deposite { get; set; }

    public int? Area { get; set; }

    public decimal? Square { get; set; }

    public string? Description { get; set; }

    public int? MaxOccupancy { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? LandlordId { get; set; }

    public string? AddressId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual Landlord? Landlord { get; set; }

    public virtual ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();

    public virtual ICollection<RoomPrice> RoomPrices { get; set; } = new List<RoomPrice>();

    public virtual ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
