using FluentValidation;
using Prescriptions.Application.Dtos.Prescriptions;

namespace Prescriptions.Infrastructure.Implementation.Validators;

public class PrescriptionUpdateDtoValidator : AbstractValidator<PrescriptionUpdateDto>
{
    public PrescriptionUpdateDtoValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid prescription status.");

        RuleFor(x => x.ExpirationDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.ExpirationDate.HasValue)
            .WithMessage("Expiration date must be in the future.");
    }
}