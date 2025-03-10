using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.CustomDataAnnotation
{
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required.");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!Regex.IsMatch(password, @"[\W_]"))
            {
                return new ValidationResult("Password must contain at least one special character.");
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return new ValidationResult("Password must contain at least one number.");
            }
            return ValidationResult.Success;
        }
    }
}
