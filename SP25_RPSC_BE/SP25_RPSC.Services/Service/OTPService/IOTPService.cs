using SP25_RPSC.Data.Models.OTPModel;


namespace SP25_RPSC.Services.Service.OTPService
{
    public interface IOTPService
    {
        //bool SendOtp(string phone, string otp);
        Task VerifyOTPToActivateAccount(OTPVerifyRequestModel model);
        Task VerifyOTPForResetPassword(OTPVerifyRequestModel model);
    }
}
