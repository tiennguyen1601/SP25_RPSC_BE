using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.CustomDataAnnotation
{
    public class ConfirmPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var passwordProperty = validationContext.ObjectType.GetProperty("Password");
            if (passwordProperty == null)
            {
                return new ValidationResult("Password property not found.");
            }

            var passwordValue = passwordProperty.GetValue(validationContext.ObjectInstance) as string;
            var confirmPasswordValue = value as string;

            if (passwordValue != confirmPasswordValue)
            {
                return new ValidationResult("Confirm password does not match.");
            }

            return ValidationResult.Success;
        }
    }
}
