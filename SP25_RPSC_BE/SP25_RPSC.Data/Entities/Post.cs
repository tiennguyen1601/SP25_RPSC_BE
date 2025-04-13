using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Post
{
    public string PostId { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? RentalRoomId { get; set; }

    public string? UserId { get; set; }

    public virtual Room? RentalRoom { get; set; }

    public virtual ICollection<RoommateRequest> RoommateRequests { get; set; } = new List<RoommateRequest>();

    public virtual User? User { get; set; }
}
