using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SP25_RPSC.Data.Models.UserModels.Request
{
    public class UpdateLandlordProfileRequest
    {
        [StringLength(100, ErrorMessage = "Company name must be at most 100 characters")]
        public string? CompanyName { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Number of rooms must be at least 1")]
        //public int? NumberRoom { get; set; }

        [StringLength(50, ErrorMessage = "License number must be at most 50 characters")]
        public string? LicenseNumber { get; set; }

        [StringLength(100, ErrorMessage = "Bank name must be at most 100 characters")]
        public string? BankName { get; set; }

        [Required(ErrorMessage = "Please input your bank number!")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Bank number must be numeric!")]
        public string? BankNumber { get; set; }

        [StringLength(255, ErrorMessage = "Address must be at most 255 characters")]
        public string? Address { get; set; }

        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be 'Male', 'Female', or 'Other'")]
        public string? Gender { get; set; }

        [StringLength(100, ErrorMessage = "Full name must be at most 100 characters")]
        public string? FullName { get; set; }

        public DateOnly? Dob { get; set; }

        //[Url(ErrorMessage = "Invalid avatar URL format")]
        public IFormFile? Avatar { get; set; }

        public List<IFormFile> BusinessImages { get; set; }
    }

}
