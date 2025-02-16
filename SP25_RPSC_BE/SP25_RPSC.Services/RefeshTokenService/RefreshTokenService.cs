using SP25_RPSC.Data.Models.RefreshTokenModel.Request;
using SP25_RPSC.Data.Models.RefreshTokenModel.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.JWTService;
using SP25_RPSC.Services.Utils.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.RefeshTokenService
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IJWTService _jWTService;

        public RefreshTokenService(IUnitOfWork unitOfWork,
            IJWTService jWTService
            )
        {
            _unitOfWork = unitOfWork;
            _jWTService = jWTService;
        }

        public async Task<RefreshTokenResModel> RefreshToken(RefreshTokenReqModel refreshTokenReqModel)
        {
            var currRefreshToken = await _unitOfWork.RefreshTokenRepository.GetByRefreshToken(refreshTokenReqModel.RefreshToken);

            if (currRefreshToken == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Refresh token not found");
            }

            if (currRefreshToken.ExpiredAt < DateTime.Now)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Refresh token is expired");
            }

            var currUser = currRefreshToken.User;

            if (currUser != null)
            {
                var token = _jWTService.GenerateJWT(currUser);

                var newRefreshToken = _jWTService.GenerateRefreshToken();

                currRefreshToken.Token = newRefreshToken;
                currRefreshToken.ExpiredAt = DateTime.Now.AddDays(1);
                await _unitOfWork.RefreshTokenRepository.Update(currRefreshToken);

                return new RefreshTokenResModel
                {
                    accessToken = token,
                    newRefreshToken = newRefreshToken
                };
            }
            else
            {
                throw new ApiException(HttpStatusCode.NotFound, "User does not exist");
            }
        }
    }
}
