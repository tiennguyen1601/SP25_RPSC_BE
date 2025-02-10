using SP25_RPSC.Data.Models.UserModels.Request;
using SP25_RPSC.Data.Models.UserModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<UserLoginResModel> Login(UserLoginReqModel userLoginReqModel);

        Task Register(UserRegisterReqModel userRegisterReqModel);
    }
}
