using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomStay
{
    public class ListRoommateRes
    {
        public RoomStayInfo? RoomStay { get; set; }
        public List<RoommateInfo> RoommateList{ get; set; } = new();
        public int TotalRoomer { get; set; }
    }

    public class RoomStayInfo
    {
        public string RoomStayId { get; set; } = null!;
        public string? RoomId { get; set; }
        public string? LandlordId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
    }

    public class RoommateInfo
    {
        public string? CustomerId { get; set; }
        public string? RoomerType { get; set; }
        public string? CustomerType { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Avatar { get; set; }
        public string? Preferences { get; set; }
        public string? LifeStyle { get; set; }
        public string? BudgetRange { get; set; }
        public string? PreferredLocation { get; set; }
        public string? Requirement { get; set; }
        public string? UserId { get; set; }
    }
}
