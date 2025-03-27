using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.UserModels.Request;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Security;
using SP25_RPSC.Services.Service.EmailService;
using SP25_RPSC.Services.Service.JWTService;
using SP25_RPSC.Services.Service.OTPService;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Services.Utils.Email;
using SP25_RPSC.Services.Utils.OTPs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IDecodeTokenHandler _decodeToken;
        private readonly IJWTService _jWTService;
        private readonly IOTPService _oTPService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthenticationService(IUnitOfWork unitOfWork,
            IDecodeTokenHandler decodeToken,
            IJWTService jWTService,
            IOTPService oTPService,
            IMapper mapper,
            IEmailService emailService
            )
        {
            _unitOfWork = unitOfWork;
            _decodeToken = decodeToken;
            _jWTService = jWTService;
            _oTPService = oTPService;
            _mapper = mapper;
            _emailService = emailService;
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

                    var refreshToken = _jWTService.GenerateRefreshToken();

                    var newRefreshToken = new RefreshToken
                    {
                        Token = refreshToken,
                        ExpiredAt = DateTime.Now.AddDays(1),
                        UserId = currentUser.UserId
                    };

                    await _unitOfWork.RefreshTokenRepository.Add(newRefreshToken);

                    string? roleUserId = null;
                    if (currentUser.Role.RoleName == "Landlord" && currentUser.Landlords.Any())
                    {
                        roleUserId = currentUser.Landlords.FirstOrDefault()?.LandlordId;
                    }
                    else if (currentUser.Role.RoleName == "Customer" && currentUser.Customers.Any())
                    {
                        roleUserId = currentUser.Customers.FirstOrDefault()?.CustomerId;
                    }

                    var userLoginRes = new UserLoginResModel
                    {
                        UserId = currentUser.UserId,
                        PhoneNumber = currentUser.PhoneNumber,
                        Email = currentUser.Email,
                        FullName = currentUser.FullName,
                        Role = currentUser.Role.RoleName,
                        Token = token,
                        RefreshToken = refreshToken,
                        RoleUserId = roleUserId
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

        public async Task Logout(string refreshToken)
        {
            var currRefreshToken = await _unitOfWork.RefreshTokenRepository.GetByRefreshToken(refreshToken);

            if (currRefreshToken == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Refresh token does not exist");
            }

            await _unitOfWork.RefreshTokenRepository.Remove(currRefreshToken);
            await _unitOfWork.SaveAsync();
        }

        public async Task Register(UserRegisterReqModel model)
        {
            var existMail = await _unitOfWork.UserRepository.GetUserByEmail(model.Email);
            if (existMail != null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The email has already registered by other account!");
            }

            var existPhone = await _unitOfWork.UserRepository.GetUserByPhoneNumber(model.PhoneNumber);
            if (existPhone != null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The Phone has already registered by other account!");
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

            var htmlBody = EmailTemplate.VerifyEmailOTP(model.Email, newOtp);
            bool sendEmailSuccess = await _emailService.SendEmail(model.Email, "Verify Email", htmlBody);

            if (!sendEmailSuccess)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "An error occurred while sending email!");
            }

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
            await _unitOfWork.SaveAsync();

        }

        public async Task ForgotPassword(string email)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Email is not existed");
            }

            var otpCode = OTPGeneration.CreateNewOTPCode();

            var emailBody = EmailTemplate.OTPForForgotPassword(email, otpCode);
            bool sendEmailSuccess = await _emailService.SendEmail(email, "Confirm reset password", emailBody);

            if (!sendEmailSuccess)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Fail to send email");
            }

            Otp newOtp = new Otp
            {
                Id = Guid.NewGuid().ToString(),
                Code = otpCode,
                CreatedBy = user.UserId,
                CreatedAt = DateTime.Now,
                IsUsed = false
            };
            await _unitOfWork.OTPRepository.Add(newOtp);
            await _unitOfWork.SaveAsync();
        }

        public async Task ResetPassword(ResetPasswordRequest model)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(model.Email);
            if (user == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Email is not existed");
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Password must be at least 6 characters long.");
            }

            user.Password = PasswordHasher.HashPassword(model.NewPassword);
            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveAsync();
        }


        public async Task RegisterCustomer(CustomerRegisterReqModel model)
        {
            var existMail = await _unitOfWork.UserRepository.GetUserByEmail(model.Email);
            if (existMail != null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The email has already registered by other account!");
            }

            var newOtp = OTPGeneration.CreateNewOTPCode();
            var userId = Guid.NewGuid().ToString();

            User newUser = _mapper.Map<User>(model);
            newUser.UserId = userId;
            newUser.Password = PasswordHasher.HashPassword(newUser.Password);
            newUser.Role = (await _unitOfWork.RoleRepository.Get(x => x.RoleName.Equals(RoleEnums.Customer.ToString()))).FirstOrDefault();
            newUser.Status = StatusEnums.Pending.ToString();
            newUser.CreateAt = DateTime.Now;
            newUser.UpdateAt = DateTime.Now;
            //newUser.Avatar = model.Avatar

            var cusType = CustomerTypeEnums.Student.ToString();
            if (model.CustomerType != null) { cusType = model.CustomerType.ToString(); }

            Customer newCus = new Customer();
            newCus.CustomerId = Guid.NewGuid().ToString();
            newCus.UserId = userId;
            newCus.CustomerType = cusType;
            newCus.Status = StatusEnums.Pending.ToString();     

            var htmlBody = EmailTemplate.VerifyEmailOTP(model.Email, newOtp);
            bool sendEmailSuccess = await _emailService.SendEmail(model.Email, "Verify Email", htmlBody);

            if (!sendEmailSuccess)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "An error occurred while sending email!");
            }

            Otp newOTPCode = new Otp()
            {
                Id = Guid.NewGuid().ToString(),
                Code = newOtp,
                CreatedBy = newUser.UserId,
                CreatedAt = DateTime.Now,
                IsUsed = false,
            };
            await _unitOfWork.UserRepository.Add(newUser);
            await _unitOfWork.CustomerRepository.Add(newCus);
            await _unitOfWork.OTPRepository.Add(newOTPCode);
            await _unitOfWork.SaveAsync();
        }



    }
}
