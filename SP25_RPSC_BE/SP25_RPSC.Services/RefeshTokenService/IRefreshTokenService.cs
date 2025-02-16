using SP25_RPSC.Data.Models.RefreshTokenModel.Request;
using SP25_RPSC.Data.Models.RefreshTokenModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.RefeshTokenService
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenResModel> RefreshToken(RefreshTokenReqModel refreshTokenReqModel);
    }
}
