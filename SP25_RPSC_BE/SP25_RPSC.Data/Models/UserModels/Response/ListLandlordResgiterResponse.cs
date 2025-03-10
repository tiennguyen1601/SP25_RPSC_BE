using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.UserModels.Response
{
    public class GetAllLandlordRegisterResponseModel
    {
        public int TotalUser { get; set; }

        public List<ListLandlordResgiterResponse> Landlords { get; set; }
    }
    public class ListLandlordResgiterResponse
    {
        public string LandlordId { get; set; }
        //public string CompanyName { get; set; }
        //public int NumberRoom { get; set; }
        //public string LicenseNumber { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        //public string Address { get; set; }
        public string PhoneNumber { get; set; }
        //public string Gender { get; set; }
        //public string Avatar { get; set; }
        public string UserStatus { get; set; }
        //public List<string> BusinessImageUrls { get; set; } = new List<string>();
    }
}
