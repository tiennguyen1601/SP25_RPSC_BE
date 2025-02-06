using SP25_RPSC.Data.Models.DecodeTokenModel;
using SP25_RPSC.Services.JWTService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Utils.DecodeTokenHandler
{
    public interface IDecodeTokenHandler
    {
        TokenModel decode(string token);
    }

    public class DecodeTokenHandler : IDecodeTokenHandler
    {
        private readonly IJWTService _jWTService;

        public DecodeTokenHandler(IJWTService jWTService)
        {
            _jWTService = jWTService;
        }
        public TokenModel decode(string token)
        {
            var roleName = _jWTService.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType);
            var userId = _jWTService.decodeToken(token, "userid");
            var phoneNumber = _jWTService.decodeToken(token, "phoneNumber");

            return new TokenModel(userId, roleName, phoneNumber);
        }
    }
}
