using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerModel.Response
{
    public class ListRequestSharingRes
    {
        public List<RequestSharingInfo> RequestSharingList{ get; set; } = new();
        public int TotalRequestSharing { get; set; }
    }

    public class RequestSharingInfo 
    {
        public string? Message { get; set; }
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
        public string? CustomerId { get; set; }
        public string? UserId { get; set; }
    }
}
