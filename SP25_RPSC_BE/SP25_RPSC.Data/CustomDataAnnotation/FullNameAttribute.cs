using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.CustomDataAnnotation
{
    public class FullNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var fullName = value as string;

            if (string.IsNullOrEmpty(fullName))
            {
                return new ValidationResult("Please input your full name!");
            }

            if (fullName.Length <= 4)
            {
                return new ValidationResult("Your full name must be larger than 4 characters.");
            }

            if (!Regex.IsMatch(fullName, @"^[A-Z]"))
            {
                return new ValidationResult("The first letter of your full name must be uppercase.");
            }

            return ValidationResult.Success;
        }
    }
}
