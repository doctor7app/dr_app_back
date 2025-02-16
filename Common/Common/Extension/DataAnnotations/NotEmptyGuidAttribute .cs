using System.ComponentModel.DataAnnotations;

namespace Common.Extension.DataAnnotations;

public class NotEmptyGuidAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is Guid guidValue && guidValue == Guid.Empty)
        {
            return new ValidationResult(ErrorMessage ?? "The GUID cannot be empty.");
        }

        return ValidationResult.Success;
    }
}