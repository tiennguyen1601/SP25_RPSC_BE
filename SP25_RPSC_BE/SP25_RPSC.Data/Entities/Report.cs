using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class Report
{
    public string ReportId { get; set; } = null!;

    public string? Description { get; set; }

    public string? Type { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Status { get; set; }

    public string? ReporterId { get; set; }

    public string? ReportedUserId { get; set; }

    public string? RentalRoomId { get; set; }

    public virtual Room? RentalRoom { get; set; }

    public virtual User? ReportedUser { get; set; }

    public virtual User? Reporter { get; set; }
}
