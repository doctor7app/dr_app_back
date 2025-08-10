using FluentValidation;
using Prescriptions.Application.Dtos.Items;

namespace Prescriptions.Infrastructure.Implementation.Validators;

public class PrescriptionItemUpdateDtoValidator : AbstractValidator<PrescriptionItemUpdateDto>
{
    public PrescriptionItemUpdateDtoValidator()
    {
        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required.");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("Frequency is required.");

        RuleFor(x => x.Duration)
            .NotEmpty().WithMessage("Duration is required.");

        RuleFor(x => x.Instructions)
            .MaximumLength(1000).WithMessage("Instructions cannot exceed 1000 characters.");

        RuleFor(x => x.IsPrn)
            .NotNull().WithMessage("IsPrn must be specified.");
    }
}