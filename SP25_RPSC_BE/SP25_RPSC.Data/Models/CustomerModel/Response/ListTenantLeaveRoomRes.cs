using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerModel.Response
{
    public class ListTenantLeaveRoomRes
    {
        public List<TenantMoveOutRes> LeaveRoomRequestList { get; set; } = new();
    }

    public class TenantMoveOutRes
    {
        public string Cmoid { get; set; } = null!;
        public string? UserMoveId { get; set; }
        public string? UserDepositeId { get; set; }
        public string? RoomStayId { get; set; }
        public DateTime? DateRequest { get; set; }
        public int? Status { get; set; }
        public TenantInfo TenantInfo { get; set; }
        public DesignatedInfo DesignatedInfo { get; set; }
    }

    public class TenantInfo
    {
        public string? UserId { get; set; }
        public string? CustomerId { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public string? Avatar { get; set; }
        public string RoomId { get; set; } = null!;
        public string? RoomNumber { get; set; }
        public string? Title { get; set; }
    }

    public class DesignatedInfo 
    {
        public string? DesignatedId { get; set; }
        public string? CustomerId { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
    }
}
