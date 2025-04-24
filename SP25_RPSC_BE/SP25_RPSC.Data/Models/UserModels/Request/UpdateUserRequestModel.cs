using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SP25_RPSC.Data.Models.UserModels.Request
{
    public class UpdateUserRequestModel
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateOnly? Dob { get; set; }
        public IFormFile? Avatar { get; set; }
    }

    public class UpdateCustomerRequestModel
    {
        public string Preferences { get; set; }
        public string LifeStyle { get; set; }
        public string BudgetRange { get; set; }
        public string PreferredLocation { get; set; }
        public string Requirement { get; set; }
        public string CustomerType { get; set; }
    }

}
