using System.ComponentModel.DataAnnotations;

namespace Common.Extension.DataAnnotations;

public class IntRangeAttribute : ValidationAttribute
{
    private readonly int _min;
    private readonly int _max;

    public IntRangeAttribute(int min, int max)
    {
        _min = min;
        _max = max;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is int intValue)
        {
            if (intValue < _min || intValue > _max)
            {
                return new ValidationResult(ErrorMessage ?? $"The value must be between {_min} and {_max}.");
            }
        }

        return ValidationResult.Success;
    }
}