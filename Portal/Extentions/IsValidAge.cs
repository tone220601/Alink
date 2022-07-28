using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Extentions
{
    public class IsValidAge : ValidationAttribute
    {
        public const string DefaultErrorMessage = "A valid date of birth is required";
        public const string DefaultToOldErrorMessage = "You are entitled to free travel from the age of 60.";
        public const string DefaulttoyoungErrorMessage = "You have to be at least 16 to purchase a aLink card.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var date = (DateTime)value;
                var age = DateTime.Today.Year - date.Year - 1;

                if (date.Year == 1 || date >= DateTime.Today)
                {
                    return new ValidationResult(DefaultErrorMessage);
                }
                if (age == 15 && date.DayOfYear < DateTime.Today.DayOfYear || age < 15)
                {
                    return new ValidationResult(DefaulttoyoungErrorMessage);
                }
                if (age == 59)
                {
                    if (date.DayOfYear >= DateTime.Today.DayOfYear)
                    {
                        return new ValidationResult(DefaultToOldErrorMessage);
                    }
                    return ValidationResult.Success;
                }

                if (age > 60)
                {
                    
                    return new ValidationResult(DefaultToOldErrorMessage);
                }
              
                    return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(ErrorMessage); ;
            }
        }
    }
}