using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Utils.Email
{
    public static class EmailTemplate
    {
        public const string logoUrl = "https://res.cloudinary.com/dzoxs1sd7/image/upload/v1741244678/easyroomie-sign.png";
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

        public static string LandlordRejection(string fullname, string reason)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
                    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
                        <h2 style='color: #dc3545; text-align: center;'>Xin lỗi, {fullname}!</h2>
                        <p style='font-size: 16px;'>Yêu cầu đăng ký của bạn đã bị từ chối.</p>
                        <p><strong>Lý do từ chối:</strong> {reason}</p>
                        <p>Vui lòng kiểm tra lại thông tin đăng ký hoặc liên hệ hỗ trợ để biết thêm chi tiết.</p>
                        <a href='mailto:easyroomie.rpsc@gmail.com' style='display: inline-block; padding: 10px 20px; background: #dc3545; color: #fff; text-decoration: none; border-radius: 5px;'>Liên hệ hỗ trợ</a>
                        <p>Cảm ơn bạn đã quan tâm đến dịch vụ của chúng tôi.</p>
                        <p><strong>EasyRoomie</strong></p>
                    </div>
                </div>";
        }


        public static string OTPForForgotPassword(string fullname, string OTP)
        {
            var html = $@"<div style='font-family: Arial, sans-serif; color: #333; line-height: 1.6; background-color: #f9f9f9; padding: 30px;'>
     <div style='background: #ffffff; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1); padding: 25px; max-width: 600px; margin: auto;'>
         <h2 style='color: #333; text-align: center; margin-bottom: 10px;'>Password Reset Request</h2>
         <hr style='border: none; border-bottom: 2px solid #eee; margin: 20px 0;'/>
         <p style='font-size: 16px;'>Dear <strong>{fullname}</strong>,</p>
         <p style='font-size: 14px;'>
            We received a request to reset your password.<br/>
            Your OTP (One-Time Password) for password reset is:<br/>
            <span style='display: inline-block; background: #007bff; color: #fff; padding: 10px 20px; font-size: 20px; font-weight: bold; border-radius: 5px; letter-spacing: 2px;'>{OTP}</span>
         </p>
         <p style='font-size: 14px;'>
            Please enter this code to proceed with resetting your password.<br/>
            This OTP is valid for a limited time.
         </p>
         <p style='font-size: 14px;'>
            If you did not request a password reset, please ignore this email or 
            <a href='' style='color: #0066cc; text-decoration: none; font-weight: bold;'>contact support</a> immediately.
         </p>
         <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>
         <p style='font-size: 14px;'>This is an automated email, please do not reply.</p>
         <p style='font-size: 14px;'>Best regards,<br/><strong>Workshopista</strong></p>
     </div>
</div>";

            return html;
        }

        public static string EmailAfterPaymentTemplate(string landlordName, string contractLink, string subject)
        {
            string htmlTemplate = @"<head>    
    <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
    <title>
        {TITLE}
    </title>
    <style type=""text/css"">
    html {
        background-color: #FFF;
    }
    body {
        font-size: 120%;
        background-color: #f7f7f7;
        border-radius: 5px;
        font-family: Arial, sans-serif;
        color: #333;
    }
    .logo {
        text-align: center;
        padding: 2% 0;
    }
    .logo img {
        width: 40%;
        height: 35%;
    }
    .title {
        padding: 2% 5%;
        text-align: center; 
        background-color: #4CAF50; 
        color: #FFF;
        border-radius: 5px 5px 0 0;
    }
    .score-details {
        padding: 3% 5%;
        text-align: left;
        background-color: #FFF;
        border-radius: 0 0 5px 5px;
    }
    .score-details h3 {
        font-size: 150%; /* Tăng kích thước font của Score */
        color: #4CAF50;
    }
    .feedback {
        margin-top: 20px;
        font-style: italic;
        color: #555;
        font-size: 130%; /* Tăng kích thước font của Feedback */
    }
    .footer {
        padding: 2% 5%;
        text-align: center; 
        font-size: 80%; 
        opacity: 0.8; 
    }
</style>
</head>
<body>
    <div class=""logo"">
        <img src=""{LOGO_URL}"" alt=""Company Logo""/>
    </div>
    <div class=""title"">
        <h2>Hello, {STAFF_NAME}</h2>
        <p>Thank you for purchasing our service package. We hope you have a great experience in the future!</p>
        <p>This is the contract link for the service: <a href=""{CONTRACT_LINK}"">{CONTRACT_LINK}</a></p>
        <p>If you need any assistance, please contact our customer support:</p>
        <ul>
            <li>Phone: +84-XXX-XXX-XXX</li>
            <li>Email: easyroomie@gmail.com</li>
            <li>Hotline: 1800-XXX-XXX</li>
        </ul>
        <p>We usually respond within 24 hours to any inquiries or support requests.</p>
        <p>Please remember to renew your service as needed. For any important notices regarding your contract and service, feel free to reach out to us.</p>
        <p>We truly appreciate your business and look forward to serving you!</p>
    </div>
    <div class=""footer"">
        <p>This is an automated email. Please do not reply.</p>
        <p>17th Floor, LandMark 81, 208 Nguyen Huu Canh Street, Binh Thanh District, Ho Chi Minh City, 700000, Vietnam</p>
    </div>
</body>
</html>
";

            htmlTemplate = htmlTemplate.Replace("{STAFF_NAME}", landlordName)
                                        .Replace("{LOGO_URL}", logoUrl)
                                        .Replace("{TITLE}", subject)
                                        .Replace("{CONTRACT_LINK}", contractLink);

            return htmlTemplate;
        }


        public static string BookingSuccess(string fullname, string roomAddress, string landlordName, string startDate)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
                    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
                        <h2 style='color: #28a745; text-align: center;'>Chúc mừng, {fullname}!</h2>
                        <p style='font-size: 16px;'>Bạn đã được chấp nhận thuê phòng tại <strong>{roomAddress}</strong>.</p>
                        <p><strong>Chủ nhà:</strong> {landlordName}</p>
                        <p><strong>Ngày bắt đầu hợp đồng:</strong> {startDate}</p>
                        <p>Chúng tôi sẽ gửi hợp đồng thuê phòng để bạn ký trong thời gian sớm nhất.</p>
                        <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi. Chúc bạn có một trải nghiệm tốt!</p>
                        <p><strong>EasyRoomie</strong></p>
                    </div>
                </div>";
        }


        public static string BookingFailure(string fullname, string roomAddress, string landlordName, string reason)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <h2 style='color: #dc3545; text-align: center;'>Xin lỗi, {fullname}!</h2>
            <p style='font-size: 16px;'>Yêu cầu thuê phòng của bạn tại <strong>{roomAddress}</strong> đã bị từ chối.</p>
            <p><strong>Chủ nhà:</strong> {landlordName}</p>
            <p><strong>Lý do từ chối:</strong> {reason}</p>
            <p>Chúng tôi rất tiếc về điều này. Bạn có thể tìm các phòng khác phù hợp hơn trên nền tảng của chúng tôi.</p>
            <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>
            <p><strong>EasyRoomie</strong></p>
        </div>
    </div>";
        }


    }
}
