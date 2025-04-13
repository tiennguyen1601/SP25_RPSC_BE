using SP25_RPSC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerModel.Response
{
    public class ListLeaveRoomRes
    {
        public List<CustomerMoveOutRes> LeaveRoomRequestList { get; set; } = new();
    }

    public class CustomerMoveOutRes
    {
        public string Cmoid { get; set; } = null!;
        public string? UserMoveId { get; set; }
        public string? RoomStayId { get; set; }
        public DateTime? DateRequest { get; set; }
        public int? Status { get; set; }
        public MemberInfo MemberInfo { get; set; }  
    }

    public class MemberInfo
    {
        public string? UserId { get; set; }
        public string? CustomerId { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Avatar { get; set; }
    }
}
