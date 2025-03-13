using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Utils.Email
{
    public static class EmailTemplate
    {
        public static string VerifyEmailOTP(string fullname, string OTP)
        {
            var html = $@"<div style='font-family: Arial, sans-serif; color: #333; line-height: 1.6; background-color: #f9f9f9; padding: 30px;'>
     <div style='background: #ffffff; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1); padding: 25px; max-width: 600px; margin: auto;'>
         <h2 style='color: #333; text-align: center; margin-bottom: 10px;'>Email Verification</h2>
         <hr style='border: none; border-bottom: 2px solid #eee; margin: 20px 0;'/>
         <p style='font-size: 16px;'>Dear <strong>{fullname}</strong>,</p>
         <p style='font-size: 14px;'>
            Your verification code (OTP) is:<br/>
            <span style='display: inline-block; background: #d9534f; color: #fff; padding: 10px 20px; font-size: 20px; font-weight: bold; border-radius: 5px; letter-spacing: 2px;'>{OTP}</span>
         </p>
         <p style='font-size: 14px;'>
            Please enter this code to verify your email address and activate your account.<br/>
            Once verified, your account will be activated, and you can log in and use our services.
         </p>
         <p style='font-size: 14px;'>
            If you did not request email verification, please ignore this email or 
            <a href='' style='color: #0066cc; text-decoration: none; font-weight: bold;'>contact us</a> immediately.
         </p>
         <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>
           <p style='font-size: 14px;'>This is an automated email, please do not reply.</p>
             <p style='font-size: 14px;'>Best regards,<br/><strong>Workshopista</strong></p>
     </div>
</div>";

            return html;
        }


        public static string LandlordApproval(string fullname)
        {
            return $@"
        <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
            <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
                <h2 style='color: #28a745; text-align: center;'>Chúc mừng, {fullname}!</h2>
                <p style='font-size: 16px;'>Tài khoản của bạn đã được phê duyệt.</p>
                <p>Bạn có thể bắt đầu đăng tin và quản lý phòng ngay bây giờ.</p>
                <a href='https://hihi.com' style='display: inline-block; padding: 10px 20px; background: #28a745; color: #fff; text-decoration: none; border-radius: 5px;'>Đăng nhập ngay</a>
                <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
                <p><strong>EasyRoomie</strong></p>
            </div>
        </div>";
        }

        public static string LandlordRejection(string fullname)
        {
            return $@"
        <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
            <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
                <h2 style='color: #dc3545; text-align: center;'>Xin lỗi, {fullname}!</h2>
                <p style='font-size: 16px;'>Yêu cầu đăng ký của bạn đã bị từ chối.</p>
                <p>Vui lòng kiểm tra lại thông tin đăng ký hoặc liên hệ hỗ trợ để biết thêm chi tiết.</p>
                <a href='mailto:easyroomie.rpsc@gmail.com' style='display: inline-block; padding: 10px 20px; background: #dc3545; color: #fff; text-decoration: none; border-radius: 5px;'>Liên hệ hỗ trợ</a>
                <p>Cảm ơn bạn đã quan tâm đến dịch vụ của chúng tôi.</p>
                <p><strong>EasyRoomie</strong></p>
            </div>
        </div>";
        }

    }
}
