using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.UserModels.Request;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.JWTService;
using SP25_RPSC.Services.Security;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IDecodeTokenHandler _decodeToken;
        private readonly IJWTService _jWTService;

        public AuthenticationService(IUnitOfWork unitOfWork,
            IDecodeTokenHandler decodeToken,
            IJWTService jWTService
            )
        {
            _unitOfWork = unitOfWork;
            _decodeToken = decodeToken;
            _jWTService = jWTService;
        }
        public async Task<UserLoginResModel> Login(UserLoginReqModel userLoginReqModel)
        {
            var currentUser = await _unitOfWork.UserRepository.GetUserByPhoneNumber(userLoginReqModel.PhoneNumber);

            if (currentUser != null)
            {
                if (currentUser.Status.Equals(StatusEnums.Inactive.ToString())) throw new ApiException(HttpStatusCode.BadRequest, "This account has been deactivated");

                if (PasswordHasher.VerifyPassword(userLoginReqModel.Password, currentUser.Password))
                {
                    var token = _jWTService.GenerateJWT(currentUser);

                    var userLoginRes = new UserLoginResModel
                    {
                        UserId = currentUser.UserId,
                        PhoneNumber = currentUser.PhoneNumber,
                        Email = currentUser.Email,
                        Role = currentUser.Role.RoleName,
                        Token = token
                    };

                    return userLoginRes;
                }
                else
                {
                    throw new ApiException(HttpStatusCode.BadRequest, "Incorrect password");
                }
            }
            else
            {
                throw new ApiException(HttpStatusCode.NotFound, "User does not exist");
            }
        }
    }
}
