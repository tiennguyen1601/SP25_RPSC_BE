using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Models.RoomModel.RoomResponseModel;

namespace SP25_RPSC.Data.Models.CustomerModel.Response
{
    public class GetLandlordByUserIdResponseModel
    {
        public UserResponseModel User { get; set; }
        public LandlordResponseUptModel Landlord { get; set; }
    }
    public class LandlordResponseUptModel
    {
        public string LandlordId { get; set; }
        public string? CompanyName { get; set; }
        public int? NumberRoom { get; set; }
        public string? LicenseNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? Template { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<string> BusinessImages { get; set; } = new List<string>();
    }


}
