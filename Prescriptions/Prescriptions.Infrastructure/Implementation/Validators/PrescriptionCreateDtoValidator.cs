using FluentValidation;
using Prescriptions.Application.Dtos.Prescriptions;

namespace Prescriptions.Infrastructure.Implementation.Validators;

public class PrescriptionCreateDtoValidator : AbstractValidator<PrescriptionCreateDto>
{
    public PrescriptionCreateDtoValidator()
    {
        RuleFor(x => x.Notes)
            .NotEmpty().WithMessage("Notes are required.")
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");

        RuleFor(x => x.ConsultationType)
            .IsInEnum().WithMessage("Invalid consultation type.");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required.");

        RuleFor(x => x.ConsultationId)
            .NotEmpty().WithMessage("ConsultationId is required.");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required.");

        RuleFor(x => x.ExpirationDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.ExpirationDate.HasValue)
            .WithMessage("Expiration date must be in the future.");

        RuleForEach(x => x.Items).SetValidator(new PrescriptionItemCreateDtoValidator());
    }
}