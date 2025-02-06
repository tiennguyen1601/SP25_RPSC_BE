using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Post
{
    public string PostId { get; set; } = null!;

    public string? Title { get; set; }

    public string? Descripton { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? RentalRoomId { get; set; }

    public virtual Room? RentalRoom { get; set; }

    public virtual ICollection<RoommateRequest> RoommateRequests { get; set; } = new List<RoommateRequest>();
}
