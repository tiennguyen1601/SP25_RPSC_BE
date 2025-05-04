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
             <p style='font-size: 14px;'>Best regards,<br/><strong>EasyRoomie</strong></p>
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
         <p style='font-size: 14px;'>Best regards,<br/><strong>EasyRoomie</strong></p>
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
                        <p>Hãy chủ động liên hệ với chủ trọ để biết thêm chi tiết thông tin về phòng trọ và ký hợp đồng thuê phòng nhé.</p>
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



        public static string RoomSharingRequest(string postOwnerName, string customerName, string postTitle, string roomAddress, string message)
        {   
            return $@"
            <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #4a86e8; text-align: center;'>Thông báo yêu cầu ở ghép mới</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{postOwnerName}</strong>,</p>
            <p style='font-size: 14px;'>Bạn vừa nhận được một yêu cầu ở ghép mới cho bài đăng của mình:</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #4a86e8; margin: 15px 0;'>
                <p><strong>Bài đăng:</strong> {postTitle}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Người yêu cầu:</strong> {customerName}</p>
                <p><strong>Lời nhắn:</strong> {message}</p>
            </div>
            
            <p style='font-size: 14px;'>Vui lòng đăng nhập vào tài khoản của bạn để xem chi tiết và phản hồi yêu cầu này.</p>
            

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";

        }

        public static string RoomSharingRejected(string requesterName, string postOwnerName, string postTitle, string roomAddress, string reason)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #dc3545; text-align: center;'>Thông báo từ chối yêu cầu ở ghép</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{requesterName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi rất tiếc phải thông báo rằng yêu cầu ở ghép của bạn đã bị từ chối.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #dc3545; margin: 15px 0;'>
                <p><strong>Bài đăng:</strong> {postTitle}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Chủ phòng:</strong> {postOwnerName}</p>                
                <p><strong>Lý do:</strong> {reason}</p
            </div>
            
            <p style='font-size: 14px;'>Đừng nản lòng! Bạn có thể tiếp tục tìm kiếm các lựa chọn phòng ở ghép khác phù hợp hơn trên nền tảng của chúng tôi.</p>
            <p style='font-size: 14px;'>Nếu bạn cần hỗ trợ tìm kiếm phòng phù hợp, vui lòng liên hệ với chúng tôi qua ứng dụng hoặc email.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }




        public static string RoomSharingAccepted(string requesterName, string postOwnerName, string postTitle,
                                        string roomAddress, string postOwnerPhone, string postOwnerEmail)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #28a745; text-align: center;'>Chúc mừng! Yêu cầu ở ghép đã được chấp nhận</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{requesterName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi vui mừng thông báo rằng yêu cầu ở ghép của bạn đã được chấp nhận!</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #28a745; margin: 15px 0;'>
                <p><strong>Bài đăng:</strong> {postTitle}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Chủ phòng:</strong> {postOwnerName}</p>
                <p><strong>Số điện thoại liên hệ:</strong> {postOwnerPhone}</p>
                <p><strong>Email liên hệ:</strong> {postOwnerEmail}</p>
            </div>
            
            <p style='font-size: 14px;'>Vui lòng liên hệ với chủ phòng sớm nhất có thể để thảo luận về các bước tiếp theo và sắp xếp thời gian chuyển vào.</p>
            <p style='font-size: 14px;'>Chúc bạn có trải nghiệm tuyệt vời tại nơi ở mới!</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }

        public static string RoomSharingConfirmation(string postOwnerName, string requesterName, string postTitle,
                                                   string roomAddress, string requesterPhone, string requesterEmail)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #2196F3; text-align: center;'>Xác nhận chấp nhận yêu cầu ở ghép</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{postOwnerName}</strong>,</p>
            <p style='font-size: 14px;'>Bạn đã chấp nhận <strong>{requesterName}</strong> làm người ở ghép cùng.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #2196F3; margin: 15px 0;'>
                <p><strong>Bài đăng:</strong> {postTitle}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Thông tin người ở ghép:</strong></p>
                <ul>
                    <li><strong>Họ tên:</strong> {requesterName}</li>
                    <li><strong>Số điện thoại:</strong> {requesterPhone}</li>
                    <li><strong>Email:</strong> {requesterEmail}</li>
                </ul>
            </div>
            
            <p style='font-size: 14px;'>Bài đăng tìm người ở ghép của bạn đã được đánh dấu là không còn hoạt động.</p>
            <p style='font-size: 14px;'>Chúc bạn và người ở ghép mới có trải nghiệm tốt đẹp khi ở cùng nhau!</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }

        public static string RoomSharingNotificationToLandlord(string landlordName, string postOwnerName,
                                                             string requesterName, string postTitle, string roomAddress)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #FF9800; text-align: center;'>Thông báo phòng trọ đã có người ở ghép</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{landlordName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng phòng trọ của bạn đã có thêm người ở ghép mới.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #FF9800; margin: 15px 0;'>
                <p><strong>Bài đăng:</strong> {postTitle}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Người thuê hiện tại:</strong> {postOwnerName}</p>
                <p><strong>Người ở ghép mới:</strong> {requesterName}</p>
            </div>
            
            <p style='font-size: 14px;'>{postOwnerName} (người thuê hiện tại) đã chấp nhận {requesterName} làm người ở ghép.</p>
            <p style='font-size: 14px;'>Việc chia sẻ phòng trọ này đã được xác nhận và thực hiện thông qua hệ thống của chúng tôi.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }

        public static string LeaveRoomConfirmationForMember(string memberName, string tenantName, string roomNumber, string roomAddress, string requestDate)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #2196F3; text-align: center;'>Xác nhận yêu cầu rời phòng</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{memberName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi gửi yêu cầu rời phòng của bạn đến <strong>{tenantName}</strong> (người chịu trách nhiệm của phòng).</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #2196F3; margin: 15px 0;'>
                <p><strong>Số phòng:</strong> {roomNumber}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Ngày gửi yêu cầu:</strong> {requestDate}</p>
                <p><strong>Trạng thái:</strong> <span style='color: #FF9800; font-weight: bold;'>Đang chờ xử lý</span></p>
            </div>
            
            <p style='font-size: 14px;'>Yêu cầu của bạn đã được gửi. Chúng tôi sẽ thông báo cho bạn ngay khi có xác nhận từ <strong>{tenantName}</strong>.</p>
            <p style='font-size: 14px;'>Trong thời gian chờ đợi, vui lòng tuân thủ các quy định của hợp đồng thuê phòng hiện tại.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }

        public static string MemberLeaveRoomNotificationForLandlord(string landlordName, string tenantName, string memberName, string roomNumber, string roomAddress, string requestDate)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #FF9800; text-align: center;'>Thông báo yêu cầu rời phòng</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{landlordName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng thành viên ở phòng {roomNumber} đã gửi yêu cầu rời phòng.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #FF9800; margin: 15px 0;'>
                <p><strong>Thành viên:</strong> {memberName}</p>
                <p><strong>Số phòng:</strong> {roomNumber}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Ngày yêu cầu:</strong> {requestDate}</p>
            </div>
            
            <p style='font-size: 14px;'>Yêu cầu này đã được gửi cho <strong>{tenantName}</strong> (người chịu trách nhiệm của phòng) để được xử lý.</p>
            <p style='font-size: 14px;'>Hệ thống của chúng tôi sẽ thông báo đến bạn sau khi <strong>{tenantName}</strong> đã xử lý yêu cầu này.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }

        public static string MemberLeaveRoomNotificationForTenant(string tenantName, string memberName, string roomNumber, string roomAddress, string requestDate)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #FF9800; text-align: center;'>Thông báo yêu cầu rời phòng</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{tenantName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng thành viên ở ghép cùng bạn đã gửi yêu cầu rời phòng.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #FF9800; margin: 15px 0;'>
                <p><strong>Thành viên:</strong> {memberName}</p>
                <p><strong>Số phòng:</strong> {roomNumber}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Ngày yêu cầu:</strong> {requestDate}</p>
            </div>
            
            <p style='font-size: 14px;'>Vui lòng đăng nhập vào tài khoản của bạn để xem chi tiết và xử lý yêu cầu này.</p>
            <p style='font-size: 14px;'>Hệ thống của chúng tôi sẽ thông báo đến <strong>{memberName}</strong> (người gửi yêu cầu) sau khi bạn đã xử lý yêu cầu này.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }

        public static string LeaveRoomAcceptedNotificationForMember(string memberName, string tenantName, string roomNumber, string roomAddress, string acceptDate)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #28a745; text-align: center;'>Yêu cầu rời phòng đã được chấp nhận</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{memberName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng yêu cầu rời phòng của bạn đã được <strong>{tenantName}</strong> chấp nhận.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #28a745; margin: 15px 0;'>
                <p><strong>Số phòng:</strong> {roomNumber}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Ngày chấp nhận:</strong> {acceptDate}</p>
                <p><strong>Trạng thái:</strong> <span style='color: #28a745; font-weight: bold;'>Đã chấp nhận</span></p>
            </div>
            
            <p style='font-size: 14px;'>Tài khoản của bạn đã được cập nhật và bạn không còn là thành viên của phòng này.</p>
            <p style='font-size: 14px;'>Vui lòng liên hệ với chủ nhà hoặc <strong>{tenantName}</strong> để hoàn tất các thủ tục cần thiết trước khi rời đi.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }

        public static string LeaveRoomAcceptedNotificationForTenant(string tenantName, string landlordName, string roomNumber, string roomAddress, string acceptDate)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #28a745; text-align: center;'>Yêu cầu rời phòng đã được chấp nhận</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{tenantName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng yêu cầu rời phòng của bạn đã được <strong>{landlordName}</strong> chấp nhận.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #28a745; margin: 15px 0;'>
                <p><strong>Số phòng:</strong> {roomNumber}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Ngày chấp nhận:</strong> {acceptDate}</p>
                <p><strong>Trạng thái:</strong> <span style='color: #28a745; font-weight: bold;'>Đã chấp nhận</span></p>
            </div>
            
            <p style='font-size: 14px;'>Tài khoản của bạn đã được cập nhật và bạn không còn là thành viên của phòng này.</p>
            <p style='font-size: 14px;'>Vui lòng liên hệ với chủ nhà: <strong>{landlordName}</strong> để hoàn tất các thủ tục cần thiết trước khi rời đi.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }


        public static string LeaveRoomAcceptedNotificationForLandlord(string landlordName, string memberName, string tenantName, string roomNumber, string roomAddress, string acceptDate)
        {
            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
        </div>
        <h2 style='color: #007bff; text-align: center;'>Thông báo thành viên rời phòng</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{landlordName}</strong>,</p>
        <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng yêu cầu rời phòng của thành viên <strong>{memberName}</strong> đã được <strong>{tenantName}</strong> chấp nhận.</p>
        
        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #007bff; margin: 15px 0;'>
            <p><strong>Số phòng:</strong> {roomNumber}</p>
            <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
            <p><strong>Ngày chấp nhận:</strong> {acceptDate}</p>
            <p><strong>Thành viên rời phòng:</strong> {memberName}</p>
            <p><strong>Trạng thái:</strong> <span style='color: #007bff; font-weight: bold;'>Đã chấp nhận bởi người thuê chính</span></p>
        </div>
        
        <p style='font-size: 14px;'>Hệ thống đã cập nhật thông tin và <strong>{memberName}</strong> không còn là thành viên của phòng này.</p>
        <p style='font-size: 14px;'>Vui lòng kiểm tra và cập nhật các thông tin liên quan đến hợp đồng thuê phòng nếu cần thiết.</p>
        <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
        <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
        <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }

        public static string TenantLeaveRoomAcceptedNotificationForLandlord(string landlordName, string tenantName, string roomNumber, string roomAddress, string acceptDate, string designatedName)
        {
            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
        </div>
        <h2 style='color: #007bff; text-align: center;'>Thông báo thành viên rời phòng</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{landlordName}</strong>,</p>
        <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng yêu cầu rời phòng của thành viên <strong>{tenantName}</strong> đã được chấp nhận.</p>
        
        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #007bff; margin: 15px 0;'>
            <p><strong>Số phòng:</strong> {roomNumber}</p>
            <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
            <p><strong>Ngày chấp nhận:</strong> {acceptDate}</p>
            <p><strong>Thành viên được chỉ định làm thành viên chính tiếp theo:</strong> {designatedName}</p>
            <p><strong>Trạng thái:</strong> <span style='color: #007bff; font-weight: bold;'>Đã xác nhận.</span></p>
        </div>
        
        <p style='font-size: 14px;'>Hệ thống đã cập nhật thông tin và <strong>{tenantName}</strong> không còn là thành viên của phòng này.</p>
        <p style='font-size: 14px;'>Vui lòng kiểm tra và cập nhật các thông tin liên quan đến hợp đồng thuê phòng nếu cần thiết.</p>
        <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
        <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
        <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }

        public static string TenantLeaveRoomNotificationForLandlord(string landlordName, string tenantName, string roomNumber, string roomAddress, string dateRequest, string designatedUserName)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #FF9800; text-align: center;'>Thông báo yêu cầu rời phòng từ người thuê chính</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{landlordName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng <strong>{tenantName}</strong> (người thuê chính) đã gửi yêu cầu rời phòng.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #FF9800; margin: 15px 0;'>
                <p><strong>Người thuê chính:</strong> {tenantName}</p>
                <p><strong>Số phòng:</strong> {roomNumber}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Ngày yêu cầu:</strong> {dateRequest}</p>
                <p><strong>Người được chỉ định làm người thuê chính mới:</strong> {designatedUserName}</p>
            </div>
            
            <p style='font-size: 14px;'>Yêu cầu rời phòng này đang chờ bạn xử lý. Vui lòng đăng nhập vào tài khoản của bạn để xem chi tiết và phản hồi yêu cầu.</p>
            <p style='font-size: 14px;'>Việc bạn xử lý yêu cầu này kịp thời sẽ giúp quá trình chuyển giao diễn ra thuận lợi cho tất cả các bên liên quan.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }

        public static string LeaveRoomConfirmationForTenant(string tenantName, string roomNumber, string roomAddress, string dateRequest, string designatedUserName)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #2196F3; text-align: center;'>Xác nhận yêu cầu rời phòng</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{tenantName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi xác nhận đã nhận được yêu cầu rời phòng của bạn và đã chuyển yêu cầu này đến chủ nhà để xử lý.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #2196F3; margin: 15px 0;'>
                <p><strong>Số phòng:</strong> {roomNumber}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Ngày gửi yêu cầu:</strong> {dateRequest}</p>
                <p><strong>Người được chỉ định làm người thuê chính mới:</strong> {designatedUserName}</p>
                <p><strong>Trạng thái:</strong> <span style='color: #FF9800; font-weight: bold;'>Đang chờ xử lý</span></p>
            </div>
            
            <p style='font-size: 14px;'>Yêu cầu của bạn đang được xử lý. Chúng tôi sẽ thông báo cho bạn ngay khi chủ nhà phản hồi yêu cầu này.</p>
            <p style='font-size: 14px;'>Trong thời gian chờ đợi, vui lòng tuân thủ các quy định của hợp đồng thuê phòng hiện tại và chuẩn bị các thủ tục cần thiết để bàn giao phòng.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }

        public static string TenantLeaveRoomNotificationForMembers(string memberName, string tenantName, string roomNumber, string roomAddress, string dateRequest, string designatedUserName, bool isDesignatedUser)
        {
            string designatedUserNotification = isDesignatedUser
                ? @"<p style='font-size: 14px; font-weight: bold; color: #4CAF50;'>Bạn đã được chỉ định làm người thuê chính mới cho phòng này. Sau khi yêu cầu được chấp nhận, bạn sẽ được chuyển vai trò thành người thuê chính và chịu trách nhiệm về phòng.</p>"
                : "";

            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
        <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
            </div>
            <h2 style='color: #FF9800; text-align: center;'>Thông báo yêu cầu rời phòng từ người thuê chính</h2>
            <p style='font-size: 16px;'>Xin chào <strong>{memberName}</strong>,</p>
            <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng <strong>{tenantName}</strong> (người thuê chính) đã gửi yêu cầu rời phòng.</p>
            
            <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #FF9800; margin: 15px 0;'>
                <p><strong>Người thuê chính:</strong> {tenantName}</p>
                <p><strong>Số phòng:</strong> {roomNumber}</p>
                <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
                <p><strong>Ngày yêu cầu:</strong> {dateRequest}</p>
                <p><strong>Người được chỉ định làm người thuê chính mới:</strong> {designatedUserName}</p>
            </div>
            
            {designatedUserNotification}
            
            <p style='font-size: 14px;'>Yêu cầu này đang chờ được chủ nhà xử lý. Chúng tôi sẽ thông báo đến bạn khi có bất kỳ cập nhật nào.</p>
            <p style='font-size: 14px;'>Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ trực tiếp với <strong>{tenantName}</strong> hoặc chủ nhà.</p>

            <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
            <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
            <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
        </div>
    </div>";
        }


        public static string LeaveRoomDesignatedPersonNotification(string designatedUserName, string tenantName, string roomNumber, string roomAddress, string requestDate)
        {
            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
        </div>
        <h2 style='color: #4CAF50; text-align: center;'>Thông báo: Bạn được chỉ định xử lý tiền đặt cọc và trở thành người thuê chính</h2>
        <p style='font-size: 16px;'>Xin chào <strong>{designatedUserName}</strong>,</p>
        <p style='font-size: 14px;'>Chúng tôi xin thông báo rằng <strong>{tenantName}</strong> đã chỉ định bạn là người xử lý tiền đặt cọc và trở thành người thuê chính của phòng khi họ rời đi.</p>
        
        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #4CAF50; margin: 15px 0;'>
            <p><strong>Số phòng:</strong> {roomNumber}</p>
            <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
            <p><strong>Ngày yêu cầu:</strong> {requestDate}</p>
            <p><strong>Vai trò mới của bạn:</strong> <span style='color: #4CAF50; font-weight: bold;'>Người thuê chính được chỉ định</span></p>
        </div>
        
        <p style='font-size: 14px;'>Khi trở thành người thuê chính, bạn sẽ có trách nhiệm:</p>
        <ul style='font-size: 14px;'>
            <li>Quản lý tiền đặt cọc của phòng</li>
            <li>Chịu trách nhiệm trong việc liên hệ với chủ nhà</li>
            <li>Xử lý các yêu cầu liên quan đến phòng</li>
            <li>Quản lý các thành viên ở ghép</li>
        </ul>
        
        <p style='font-size: 14px;'>Chúng tôi khuyến nghị bạn liên hệ với <strong>{tenantName}</strong> để thảo luận về quá trình bàn giao và các chi tiết liên quan đến tiền đặt cọc.</p>

        <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
        <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
        <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }

        public static string RoommateKicked(string roommateName, string tenantName, string roomNumber, string roomAddress, string reason, string moveOutDate)
        {
            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
        </div>
        <h2 style='color: #dc3545; text-align: center;'>Thông báo chấm dứt ở ghép</h2>
        <p style='font-size: 16px;'>Xin chào <strong>{roommateName}</strong>,</p>
        <p style='font-size: 14px;'>Chúng tôi thông báo rằng bạn sẽ không còn là thành viên của phòng trọ này từ ngày <strong>{moveOutDate}</strong>.</p>
        
        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #dc3545; margin: 15px 0;'>
            
            <p><strong>Số phòng:</strong> {roomNumber}</p>
            <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
            <p><strong>Chủ phòng:</strong> {tenantName}</p>                
            <p><strong>Lý do:</strong> {reason}</p>
            <p>Liên hệ với<strong> {roommateName}</strong> để xác nhận lại nếu thông tin có sai sót.</p>
        </div>
        
        <p style='font-size: 14px;'>Vui lòng thu dọn đồ đạc cá nhân và bàn giao phòng đúng hạn theo quy định.</p>
        <p style='font-size: 14px;'>Nếu bạn cần hỗ trợ tìm kiếm phòng mới, vui lòng sử dụng ứng dụng EasyRoomie hoặc liên hệ với chúng tôi.</p>
        <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
        <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
        <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }

        public static string RoommateKickedLandlordNoti(string landlordName, string tenantName,
                                                        string roommateName, string roomNumber, string roomAddress,
                                                        string reason, string moveOutDate, List<string> evidenceImageUrls = null)
        {
            // Tạo phần HTML hiển thị ảnh bằng chứng
            StringBuilder evidenceImagesHtml = new StringBuilder();
            if (evidenceImageUrls != null && evidenceImageUrls.Any())
            {
                evidenceImagesHtml.Append("<div style='margin-top: 15px;'>");
                evidenceImagesHtml.Append("<h4 style='color: #dc3545;'>Bằng chứng:</h4>");
                evidenceImagesHtml.Append("<div style='display: flex; flex-wrap: wrap; gap: 10px;'>");
                foreach (var imageUrl in evidenceImageUrls)
                {
                    evidenceImagesHtml.Append($"<div style='flex: 0 0 250px; max-width: 250px;'>");
                    evidenceImagesHtml.Append($"<img src=\"{imageUrl}\" alt=\"Bằng chứng\" style='width: 100%; max-height: 180px; object-fit: cover; border-radius: 4px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'/>");
                    evidenceImagesHtml.Append("</div>");
                }
                evidenceImagesHtml.Append("</div></div>");
            }

            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
        </div>
        <h2 style='color: #4a86e8; text-align: center;'>Thông báo: Thành viên kết thúc ở ghép</h2>
        <p style='font-size: 16px;'>Xin chào <strong>{landlordName}</strong>,</p>
        <p style='font-size: 14px;'>Chúng tôi thông báo rằng có một thành viên sẽ rời khỏi phòng trọ của bạn từ ngày <strong>{moveOutDate}</strong>.</p>
        
        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #4a86e8; margin: 15px 0;'>
            
            <p><strong>Số phòng:</strong> {roomNumber}</p>
            <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
            <p><strong>Chủ phòng:</strong> {tenantName}</p>
            <p><strong>Thành viên rời đi:</strong> {roommateName}</p>
            <p><strong>Lý do gửi cho thành viên rời đi:</strong> {reason}</p>
        </div>
        
        {evidenceImagesHtml}
        
        <p style='font-size: 14px;'>Thông báo này chỉ nhằm mục đích thông tin. Vui lòng liên hệ với người thuê chính để biết thêm chi tiết.</p>
        <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
        <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
        <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }

        public static string TenantKickedByLandlord(string tenantName, string landlordName, string roomNumber, string roomAddress, string reason, string moveOutDate, List<string> evidenceImageUrls = null)
        {
            // Tạo phần HTML hiển thị ảnh bằng chứng
            StringBuilder evidenceImagesHtml = new StringBuilder();
            if (evidenceImageUrls != null && evidenceImageUrls.Any())
            {
                evidenceImagesHtml.Append("<div style='margin-top: 15px;'>");
                evidenceImagesHtml.Append("<h4 style='color: #dc3545;'>Bằng chứng:</h4>");
                evidenceImagesHtml.Append("<div style='display: flex; flex-wrap: wrap; gap: 10px;'>");
                foreach (var imageUrl in evidenceImageUrls)
                {
                    evidenceImagesHtml.Append($"<div style='flex: 0 0 250px; max-width: 250px;'>");
                    evidenceImagesHtml.Append($"<img src=\"{imageUrl}\" alt=\"Bằng chứng\" style='width: 100%; max-height: 180px; object-fit: cover; border-radius: 4px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'/>");
                    evidenceImagesHtml.Append("</div>");
                }
                evidenceImagesHtml.Append("</div></div>");
            }

            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
        </div>
        <h2 style='color: #dc3545; text-align: center;'>Thông báo chấm dứt hợp đồng thuê phòng</h2>
        <p style='font-size: 16px;'>Xin chào <strong>{tenantName}</strong>,</p>
        <p style='font-size: 14px;'>Chúng tôi thông báo rằng chủ nhà đã quyết định chấm dứt hợp đồng thuê phòng của bạn từ ngày <strong>{moveOutDate}</strong>.</p>
        
        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #dc3545; margin: 15px 0;'>
            <p><strong>Số phòng:</strong> {roomNumber}</p>
            <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
            <p><strong>Chủ nhà:</strong> {landlordName}</p>                
            <p><strong>Lý do:</strong> {reason}</p>
            <p>Liên hệ với<strong> {landlordName}</strong> để xác nhận lại nếu thông tin có sai sót.</p
        </div>
        
        {evidenceImagesHtml}
        
        <p style='font-size: 14px;'>Vui lòng thu dọn đồ đạc cá nhân và bàn giao phòng đúng hạn theo quy định trong hợp đồng thuê phòng.</p>
        <p style='font-size: 14px;'>Nếu bạn cần hỗ trợ tìm kiếm phòng mới, vui lòng sử dụng ứng dụng EasyRoomie hoặc liên hệ với chúng tôi để được trợ giúp.</p>
        <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
        <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ thắc mắc nào hoặc cho rằng quyết định này không công bằng, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
        <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }


        public static string ExtendContractRejected(string customerName, string contractId, string reason)
        {
            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
    <div style='background-color: #ffffff; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1); padding: 25px;'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src='{logoUrl}' alt='EasyRoomie Logo' style='max-width: 180px;' />
        </div>
        <h2 style='color: #dc3545; text-align: center;'>Yêu cầu gia hạn hợp đồng bị từ chối</h2>
        <p style='font-size: 16px;'>Xin chào <strong>{customerName}</strong>,</p>
        <p style='font-size: 14px;'>Rất tiếc, yêu cầu gia hạn hợp đồng mã số <strong>{contractId}</strong> của bạn đã bị từ chối bởi chủ trọ.</p>
        
        <div style='background-color: #f8d7da; color: #721c24; padding: 15px; border-radius: 5px; border-left: 5px solid #dc3545; margin: 20px 0;'>
            <strong>Lý do từ chối:</strong>
            <p style='margin: 8px 0 0;'>{reason}</p>
        </div>

        <p style='font-size: 14px;'>Nếu bạn có thắc mắc hoặc cần trao đổi thêm, hãy liên hệ trực tiếp với chủ trọ.</p>
        <p style='font-size: 14px;'>Chúng tôi luôn sẵn sàng hỗ trợ bạn trong việc tìm kiếm và duy trì nơi ở lý tưởng.</p>

        <hr style='border: none; border-bottom: 1px solid #eee; margin: 30px 0;'/>
        <p style='font-size: 12px; color: #777;'>Đây là email tự động, vui lòng không phản hồi trực tiếp. Mọi thắc mắc xin gửi về: 
            <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #007bff;'>easyroomie.rpsc@gmail.com</a>
        </p>
        <p style='font-size: 12px; color: #777;'>Trân trọng, <br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }
        public static string NotifyLandlordForExtendRequest(string landlordName, string customerName, string contractId, int months, string message)
        {
            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 30px;'>
    <div style='background-color: #ffffff; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1); padding: 25px;'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src='{logoUrl}' alt='EasyRoomie Logo' style='max-width: 180px;' />
        </div>
        <h2 style='color: #007bff; text-align: center;'>Yêu cầu gia hạn hợp đồng</h2>
        <p style='font-size: 16px;'>Xin chào <strong>{landlordName}</strong>,</p>
        <p style='font-size: 14px;'>Người thuê <strong>{customerName}</strong> đã gửi yêu cầu gia hạn hợp đồng với mã số <strong>{contractId}</strong>.</p>

        <div style='background-color: #f1f1f1; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            <p><strong>Số tháng muốn gia hạn:</strong> {months} tháng</p>
            <p><strong>Lời nhắn từ khách thuê:</strong></p>
            <p style='font-style: italic; color: #555;'>{message}</p>
        </div>

        <p style='font-size: 14px;'>Vui lòng đăng nhập hệ thống để xem và xử lý yêu cầu này.</p>

        <hr style='border: none; border-bottom: 1px solid #eee; margin: 30px 0;'/>
        <p style='font-size: 12px; color: #777;'>Đây là email tự động, vui lòng không phản hồi. Mọi thắc mắc xin gửi về: 
            <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #007bff;'>easyroomie.rpsc@gmail.com</a>
        </p>
        <p style='font-size: 12px; color: #777;'>Trân trọng, <br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }
        public static string ExtendContractApproved(string fullname, string contractId, string newEndDate)
        {
            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 30px;'>
    <div style='background-color: #ffffff; max-width: 600px; margin: auto; border-radius: 10px; box-shadow: 0 6px 20px rgba(0,0,0,0.1); padding: 25px;'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src='{logoUrl}' alt='EasyRoomie Logo' style='max-width: 180px;' />
        </div>
        <h2 style='color: #28a745; text-align: center; margin-bottom: 10px;'>Yêu cầu gia hạn được chấp thuận</h2>
        <p style='font-size: 16px;'>Xin chào <strong>{fullname}</strong>,</p>
        <p style='font-size: 14px;'>Chúng tôi rất vui thông báo rằng yêu cầu gia hạn hợp đồng của bạn (Mã hợp đồng: <strong>{contractId}</strong>) đã được <strong>chủ nhà</strong> chấp nhận.</p>

        <div style='background-color: #e8f5e9; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            <p><strong>Ngày kết thúc mới của hợp đồng:</strong></p>
            <p style='font-size: 16px; font-weight: bold; color: #2e7d32;'>{newEndDate}</p>
        </div>

        <p style='font-size: 14px;'>Bạn có thể đăng nhập vào hệ thống EasyRoomie để xem chi tiết hợp đồng, cập nhật thông tin, hoặc liên hệ với chủ nhà nếu cần.</p>

        <hr style='border: none; border-bottom: 1px solid #eee; margin: 30px 0;' />
        <p style='font-size: 12px; color: #777;'>Đây là email tự động, vui lòng không phản hồi. Mọi thắc mắc xin gửi về: 
            <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #007bff;'>easyroomie.rpsc@gmail.com</a>
        </p>
        <p style='font-size: 12px; color: #777;'>Trân trọng, <br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }

        public static string InactivateRoommatePostByLandlord(string ownerName, string postTitle, string roomAddress, string reason)
        {
            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
        </div>
        <h2 style='color: #dc3545; text-align: center;'>Thông báo vô hiệu hóa bài đăng tìm người ở ghép</h2>
        <p style='font-size: 16px;'>Xin chào <strong>{ownerName}</strong>,</p>
        <p style='font-size: 14px;'>Chúng tôi thông báo rằng bài đăng tìm người ở ghép của bạn đã bị vô hiệu hóa bởi chủ nhà.</p>
        
        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #dc3545; margin: 15px 0;'>
            <p><strong>Bài đăng:</strong> {postTitle}</p>
            <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
            <p><strong>Lý do:</strong> {reason}</p>
        </div>
        
        <p style='font-size: 14px;'>Khi bài đăng bị vô hiệu hóa, nó sẽ không hiển thị cho người dùng khác và mọi yêu cầu ở ghép đến bài đăng này sẽ bị hủy.</p>
        <p style='font-size: 14px;'>Nếu bạn cho rằng đây là sự nhầm lẫn, vui lòng liên hệ với chủ nhà hoặc đội ngũ hỗ trợ của chúng tôi.</p>
        
        <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
        <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
        <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }

        public static string CancelRoommateRequestDueToInactivePost(string customerName, string postTitle, string roomAddress, string reason)
        {
            return $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 30px;'>
    <div style='background: #ffffff; padding: 25px; max-width: 600px; margin: auto; border-radius: 8px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src=""{logoUrl}"" alt=""EasyRoomie Logo"" style='max-width: 200px;'/> 
        </div>
        <h2 style='color: #dc3545; text-align: center;'>Thông báo hủy yêu cầu ở ghép</h2>
        <p style='font-size: 16px;'>Xin chào <strong>{customerName}</strong>,</p>
        <p style='font-size: 14px;'>Chúng tôi lấy làm tiếc thông báo rằng yêu cầu ở ghép của bạn đã bị hủy do bài đăng không còn hoạt động.</p>
        
        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #dc3545; margin: 15px 0;'>
            <p><strong>Bài đăng:</strong> {postTitle}</p>
            <p><strong>Địa chỉ phòng:</strong> {roomAddress}</p>
            <p><strong>Lý do:</strong> {reason}</p>
        </div>
        
        <p style='font-size: 14px;'>Bài đăng này đã bị vô hiệu hóa bởi chủ nhà, vì vậy tất cả các yêu cầu ở ghép liên quan đều bị hủy.</p>
        <p style='font-size: 14px;'>Đừng lo, vẫn còn rất nhiều cơ hội ở ghép khác cho bạn trên nền tảng EasyRoomie.</p>
        
        <hr style='border: none; border-bottom: 1px solid #eee; margin: 20px 0;'/>    
        <p style='font-size: 14px; color: #777;'>Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi qua email: <a href='mailto:easyroomie.rpsc@gmail.com' style='color: #4a86e8; text-decoration: none;'>easyroomie.rpsc@gmail.com</a></p>
        <p style='font-size: 14px; color: #777;'>Trân trọng,<br/><strong>Đội ngũ EasyRoomie</strong></p>
    </div>
</div>";
        }
        public static string UserInactivated(string userName, string reason)
        {
            return $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='UTF-8'>
        <title>Tài khoản đã bị vô hiệu hóa</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333333;
                max-width: 600px;
                margin: 0 auto;
                padding: 20px;
            }}
            .header {{
                background-color: #f8f9fa;
                padding: 15px;
                text-align: center;
                border-radius: 5px;
                margin-bottom: 20px;
            }}
            .content {{
                padding: 20px;
                background-color: #ffffff;
                border-radius: 5px;
                border: 1px solid #e9ecef;
            }}
            .footer {{
                text-align: center;
                margin-top: 20px;
                font-size: 12px;
                color: #6c757d;
            }}
            h1 {{
                color: #dc3545;
                font-size: 24px;
            }}
            .reason {{
                background-color: #f8f9fa;
                padding: 15px;
                border-left: 4px solid #dc3545;
                margin: 15px 0;
            }}
        </style>
    </head>
    <body>
        <div class='header'>
            <h1>Thông báo vô hiệu hóa tài khoản</h1>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{userName}</strong>,</p>
            
            <p>Chúng tôi gửi email này để thông báo rằng tài khoản của bạn trên hệ thống EasyRoomie đã bị vô hiệu hóa.</p>
            
            <div class='reason'>
                <strong>Lý do:</strong>
                <p>{reason}</p>
            </div>
            
            <p>Nếu bạn cho rằng đây là sự nhầm lẫn hoặc bạn có thắc mắc về quyết định này, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi để được giải đáp.</p>
            
            <p>Trân trọng,<br>
            Đội ngũ EasyRoomie</p>
        </div>
        <div class='footer'>
            <p>© 2025 EasyRoomie. Tất cả các quyền được bảo lưu.</p>
        </div>
    </body>
    </html>
    ";
        }

    }
}
