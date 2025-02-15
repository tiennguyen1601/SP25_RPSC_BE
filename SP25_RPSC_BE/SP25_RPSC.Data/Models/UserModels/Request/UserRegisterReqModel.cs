using SP25_RPSC.Data.CustomDataAnnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.UserModels.Request
{
    public class UserRegisterReqModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Please input your email!")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please input your password!")]
        [Length(6, 14, ErrorMessage = "Password must between 6-14 characters")]
        [PasswordComplexity]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please input your fullname!")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Please input your phone number!")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select your gender!")]
        public string? Gender { get; set; }
    }
}
