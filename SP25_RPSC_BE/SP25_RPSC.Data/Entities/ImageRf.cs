using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class ImageRf
{
    public string ImageRfid { get; set; } = null!;

    public string? ImageRfurl { get; set; }

    public string? FeedbackId { get; set; }

    public string? ReportId { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual Report? Report { get; set; }
}
