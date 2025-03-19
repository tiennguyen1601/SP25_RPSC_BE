using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Feedback
{
    public string FeedbackId { get; set; } = null!;

    public string? Description { get; set; }

    public string? Type { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Status { get; set; }

    public int? Rating { get; set; }

    public string? ReviewerId { get; set; }

    public string? RevieweeId { get; set; }

    public string? RentalRoomId { get; set; }

    public virtual ICollection<ImageRf> ImageRves { get; set; } = new List<ImageRf>();

    public virtual Room? RentalRoom { get; set; }

    public virtual User? Reviewee { get; set; }

    public virtual User? Reviewer { get; set; }
}
