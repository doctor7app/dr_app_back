using System.ComponentModel.DataAnnotations;

namespace Common.Extension.DataAnnotations;

public class DateGreaterThanOrEqualToTodayAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime dateValue)
        {
            // Compare only the date part
            if (dateValue.Date < DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "The date must be today or in the future.");
            }
        }

        return ValidationResult.Success;
    }
}