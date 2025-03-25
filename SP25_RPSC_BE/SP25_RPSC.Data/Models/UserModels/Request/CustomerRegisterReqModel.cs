using SP25_RPSC.Data.CustomDataAnnotation;
using SP25_RPSC.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace SP25_RPSC.Data.Models.UserModels.Request
{
    public class CustomerRegisterReqModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Please input your email!")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please input your password!")]
        [Length(6, 14, ErrorMessage = "Password must between 6-14 characters")]
        [PasswordComplexity]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please input your confirm password!")]
        [ConfirmPassword]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please input your fullname!")]
        [FullNameAttribute]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Customer Type is required")]
        [JsonPropertyName("customerType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CustomerTypeEnums CustomerType { get; set; }

    }
}
