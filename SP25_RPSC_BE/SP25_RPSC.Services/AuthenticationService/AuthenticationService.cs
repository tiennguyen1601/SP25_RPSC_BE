using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.UserModels.Request;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.JWTService;
using SP25_RPSC.Services.OTPService;
using SP25_RPSC.Services.Security;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Services.Utils.OTPs;
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
        private readonly IOTPService _oTPService;
        private readonly IMapper _mapper;

        public AuthenticationService(IUnitOfWork unitOfWork,
            IDecodeTokenHandler decodeToken,
            IJWTService jWTService,
            IOTPService oTPService,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _decodeToken = decodeToken;
            _jWTService = jWTService;
            _oTPService = oTPService;
            _mapper = mapper;
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

        public async Task Register(UserRegisterReqModel model)
        {
            var exist = await _unitOfWork.UserRepository.GetUserByEmail(model.Email);
            if (exist != null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The email has already registered by other account!");
            }

            var newOtp = OTPGeneration.CreateNewOTPCode();

            //bool sendOtpSuccess = _oTPService.SendOtp(model.PhoneNumber, newOtp);

            //if (!sendOtpSuccess)
            //{
            //    throw new ApiException(HttpStatusCode.BadRequest, "An error occurred while sending email!");
            //}

            User newUser = _mapper.Map<User>(model);
            newUser.UserId = Guid.NewGuid().ToString();
            newUser.Password = PasswordHasher.HashPassword(newUser.Password);
            newUser.Role = (await _unitOfWork.RoleRepository.Get(x => x.RoleName.Equals(RoleEnums.Landlord.ToString()))).FirstOrDefault();
            newUser.Status = StatusEnums.Pending.ToString();
            newUser.CreateAt = DateTime.Now;
            newUser.UpdateAt = DateTime.Now;
            //newUser.Avatar = model.Avatar

            Otp newOTPCode = new Otp()
            {
                Id = Guid.NewGuid().ToString(),
                Code = newOtp,
                CreatedBy = newUser.UserId,
                CreatedAt = DateTime.Now,
                IsUsed = false,
            };

            await _unitOfWork.UserRepository.Add(newUser);
            await _unitOfWork.OTPRepository.Add(newOTPCode);
        }
    }
}
