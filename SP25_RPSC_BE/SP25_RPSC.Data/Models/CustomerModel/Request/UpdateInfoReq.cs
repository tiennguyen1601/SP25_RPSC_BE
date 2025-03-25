using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace SP25_RPSC.Data.Models.CustomerModel.Request
{

    public class UpdateInfoReq
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Full Name can only contain letters and spaces.")]
        public string? FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters.")]
        [RegularExpression(@"^[0-9+\-\s]*$", ErrorMessage = "Phone number contains invalid characters.")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Range(typeof(DateOnly), "1900-01-01", "2024-12-31", ErrorMessage = "Date of Birth is out of valid range.")]
        public DateOnly? Dob { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
        public string? Gender { get; set; }

        [StringLength(500, ErrorMessage = "Preferences cannot exceed 500 characters.")]
        public string? Preferences { get; set; }

        [StringLength(500, ErrorMessage = "Life Style cannot exceed 500 characters.")]
        public string? LifeStyle { get; set; }

        [RegularExpression(@"^\$?\d+(-\$?\d+)?$", ErrorMessage = "Budget Range must be in a valid format (e.g., 500-1000 or 1000).")]
        [StringLength(50, ErrorMessage = "Budget Range cannot exceed 50 characters.")]
        public string? BudgetRange { get; set; }

        [StringLength(100, ErrorMessage = "Preferred Location cannot exceed 100 characters.")]
        public string? PreferredLocation { get; set; }

        [StringLength(500, ErrorMessage = "Requirements cannot exceed 500 characters.")]
        public string? Requirement { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(Student|Worker)$", ErrorMessage = "Customer Type must be either Student or Worker.")]
        public string? CustomerType { get; set; }


        public IFormFile? Avatar { get; set; }

        // Optional custom validation method
        public bool IsValid()
        {
            // Additional custom validation logic can be added here
            if (Dob.HasValue && Dob.Value > DateOnly.FromDateTime(DateTime.Now.AddYears(-16)))
            {
                return false; // Example: Ensure user is at least 16 years old
            }

            return true;
        }
    }
}
