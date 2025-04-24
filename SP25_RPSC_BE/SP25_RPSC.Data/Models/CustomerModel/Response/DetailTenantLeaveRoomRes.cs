using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerModel.Response
{
    public class DetailTenantLeaveRoomRes
    {
        public DetailTenantMoveOutRes DetailTenantMoveOutRes { get; set; } = new();
    }

    public class DetailTenantMoveOutRes
    {
        public string Cmoid { get; set; } = null!;
        public string? UserMoveId { get; set; }
        public string? UserDepositeId { get; set; }
        public string? RoomStayId { get; set; }
        public DateTime? DateRequest { get; set; }
        public int? Status { get; set; }
        public DetailTenantInfo DetailTenantInfo { get; set; }
        public DetailDesignatedInfo DetailDesignatedInfo { get; set; }
    }

    public class DetailTenantInfo
    {
        public string? UserId { get; set; }
        public string? CustomerId { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Avatar { get; set; }
        public string RoomId { get; set; } = null!;
        public string? RoomNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
    }

    public class DetailDesignatedInfo
    {
        public string? DesignatedId { get; set; }
        public string? CustomerId { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Avatar { get; set; }
    }
}
