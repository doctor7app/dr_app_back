using System.ComponentModel.DataAnnotations;

namespace Common.Extension.DataAnnotations;

public class DecimalRangeAttribute : ValidationAttribute
{
    private readonly decimal _min;
    private readonly decimal _max;

    public DecimalRangeAttribute(decimal min, decimal max)
    {
        _min = min;
        _max = max;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is decimal decimalValue)
        {
            if (decimalValue < _min || decimalValue > _max)
            {
                return new ValidationResult(ErrorMessage ?? $"The value must be between {_min} and {_max}.");
            }
        }

        return ValidationResult.Success;
    }
}