using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class CustomerRentRoomDetailRequest
{
    public string CustomerRentRoomDetailRequestId { get; set; } = null!;

    public string? Status { get; set; }

    public string? Message { get; set; }

    public string? RoomRentRequestsId { get; set; }

    public string? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual RoomRentRequest? RoomRentRequests { get; set; }
}
