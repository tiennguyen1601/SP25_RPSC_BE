using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.DecodeTokenModel
{
    public class TokenModel
    {
        public string userid { get; set; }

        public string roleName { get; set; }

        public string phoneNumber { get; set; }

        public TokenModel(string userid, string roleName, string phoneNumber)
        {
            this.userid = userid;
            this.roleName = roleName;
            this.phoneNumber = phoneNumber;
        }
    }
}
