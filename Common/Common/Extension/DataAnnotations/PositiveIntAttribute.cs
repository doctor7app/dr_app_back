using System.ComponentModel.DataAnnotations;

namespace Common.Extension.DataAnnotations;

public class PositiveIntAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success; // No validation needed if the value is null
        }

        if (value is int intValue)
        {
            if (intValue < 0)
            {
                return new ValidationResult(ErrorMessage ?? "The value must be positive.");
            }
        }

        return ValidationResult.Success;
    }
}