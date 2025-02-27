using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.CustomDataAnnotation
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneNumber = value as string;

            if (string.IsNullOrEmpty(phoneNumber))
            {
                return new ValidationResult("Phone number is required.");
            }

            if (!Regex.IsMatch(phoneNumber, @"^0\d{9,11}$"))
            {
                return new ValidationResult("Phone number must be 10-12 digits and start with 0.");
            }

            return ValidationResult.Success;
        }
    }
}
