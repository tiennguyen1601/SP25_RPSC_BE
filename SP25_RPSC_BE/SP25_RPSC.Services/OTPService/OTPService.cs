using Microsoft.Extensions.Configuration;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;


namespace SP25_RPSC.Services.OTPService
{
    public interface IOTPService
    {
        bool SendOtp(string phone, string otp);
    }
    public class OTPService : IOTPService
    {
        private readonly IConfiguration _config;

        public OTPService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool SendOtp(string phone, string otp)
        {
            try
            {
                var accountSid = _config["TwilioSettings:AccountSid"];
                var authToken = _config["TwilioSettings:AuthToken"];
                var twilioPhoneNumber = _config["TwilioSettings:TwilioPhoneNumber"];
                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                   body: $"Mã xác thực của bạn là: {otp}",
                   from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                   to: new Twilio.Types.PhoneNumber(phone)
               );

                //            var verification = VerificationResource.Create(
                //    to: "+84374676054",
                //    channel: "sms",
                //    pathServiceSid: "VA5e24cd12abed8d265d23d674591e1006"
                //);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
