using Microsoft.Extensions.Configuration;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.OTPModel;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using System.Net;


namespace SP25_RPSC.Services.Service.OTPService
{
    public class OTPService : IOTPService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        //public OTPService(IConfiguration config)
        //{
        //    _config = config ?? throw new ArgumentNullException(nameof(config));
        //}
        public OTPService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task VerifyOTPToActivateAccount(OTPVerifyRequestModel model)
        {
            User currentUser = await _unitOfWork.UserRepository.GetUserByEmail(model.Email);

            if (currentUser == null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Email bạn nhập không kết nối với tài khoản nào.");
            }

            var latestOTP = await _unitOfWork.OTPRepository.GetLatestOTP(currentUser.UserId);
            if (latestOTP != null)
            {
                //if ((DateTime.Now - latestOTP.CreatedAt).Value.TotalMinutes > 30 || latestOTP.IsUsed == true)
                //{
                //    throw new CustomException("Mã OTP đã quá thời gian hoặc đã được sử dụng. Xin vui lòng nhập mã OTP mới nhất.");
                //}

                if (latestOTP.Code.Equals(model.OTP))
                {
                    latestOTP.IsUsed = true;
                    currentUser.Status = StatusEnums.Active.ToString();
                }
                else
                {
                    throw new ApiException(HttpStatusCode.BadRequest, "Mã OTP không hợp lệ.");
                }

                await _unitOfWork.OTPRepository.Update(latestOTP);
                await _unitOfWork.UserRepository.Update(currentUser);
            }
        }

        //public bool SendOtp(string phone, string otp)
        //{
        //    try
        //    {
        //        var accountSid = _config["TwilioSettings:AccountSid"];
        //        var authToken = _config["TwilioSettings:AuthToken"];
        //        var twilioPhoneNumber = _config["TwilioSettings:TwilioPhoneNumber"];
        //        TwilioClient.Init(accountSid, authToken);

        //        var message = MessageResource.Create(
        //           body: $"Mã xác thực của bạn là: {otp}",
        //           from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
        //           to: new Twilio.Types.PhoneNumber(phone)
        //       );

        //        //            var verification = VerificationResource.Create(
        //        //    to: "+84374676054",
        //        //    channel: "sms",
        //        //    pathServiceSid: "VA5e24cd12abed8d265d23d674591e1006"
        //        //);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
    }
}
