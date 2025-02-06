using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.UserModels.Request
{
    public class UserLoginReqModel
    {
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
    }
}
