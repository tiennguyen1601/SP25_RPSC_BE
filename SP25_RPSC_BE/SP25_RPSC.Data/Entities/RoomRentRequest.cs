using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RoomRentRequest
{
    public string RoomRentRequestsId { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }

    public string? RoomId { get; set; }

    public virtual ICollection<CustomerRentRoomDetailRequest> CustomerRentRoomDetailRequests { get; set; } = new List<CustomerRentRoomDetailRequest>();

    public virtual Room? Room { get; set; }
}
