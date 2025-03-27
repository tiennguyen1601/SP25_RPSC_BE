using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerRentRoomDetail.CustomerRentRoomDetailResponse
{
    public class CustomerRequestRes
    {
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public string Preferences { get; set; }
        public string LifeStyle { get; set; }
        public string BudgetRange { get; set; }
        public string PreferredLocation { get; set; }
        public string Requirement { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }

        public string Message { get; set; }
        public int MonthWantRent { get; set; }

    }



}
