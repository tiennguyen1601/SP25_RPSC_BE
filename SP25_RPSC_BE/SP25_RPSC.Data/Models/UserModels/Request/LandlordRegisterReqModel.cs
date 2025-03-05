using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.UserModels.Request
{
    public class LandlordRegisterReqModel
    {
        [Required(ErrorMessage = "Please input your company name!")]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Please input your room number!")]
        public string? NumberRoom { get; set; }

        [Required(ErrorMessage = "Please input your license number!")]
        public string? LicenseNumber { get; set; }

        [Required(ErrorMessage = "Please upload your business license!")]
        //public string? BusinessLicense { get; set; }

        //[Required(ErrorMessage = "Please input your bank name!")]
        public string? BankName { get; set; }

        [Required(ErrorMessage = "Please input your bank number!")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Bank number must be numeric!")]
        public string? BankNumber { get; set; }

        //[Required(ErrorMessage = "Please input your template!")]
        //public string? Template { get; set; }

        //[Required(ErrorMessage = "Please input the status!")]
        //public string? Status { get; set; }

        public required List<IFormFile> WorkshopImages { get; set; }
    }
}
