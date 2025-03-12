using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.UserModels.Response
{
    public class UserLoginResModel
    {
        public string UserId { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Role { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string? RoleUserId { get; set; }
    }
}
