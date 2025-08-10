using FluentValidation;
using Prescriptions.Application.Dtos.Items;

namespace Prescriptions.Infrastructure.Implementation.Validators;

public class PrescriptionItemCreateDtoValidator : AbstractValidator<PrescriptionItemCreateDto>
{
    public PrescriptionItemCreateDtoValidator()
    {
        RuleFor(x => x.DrugName)
            .NotEmpty().WithMessage("Drug name is required.")
            .MaximumLength(255).WithMessage("Drug name cannot exceed 255 characters.");

        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required.");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("Frequency is required.");

        RuleFor(x => x.Duration)
            .NotEmpty().WithMessage("Duration is required.");

        RuleFor(x => x.MedicationType)
            .IsInEnum().WithMessage("Invalid medication type.");

        RuleFor(x => x.Route)
            .IsInEnum().WithMessage("Invalid administration route.");

        RuleFor(x => x.TimeOfDay)
            .MaximumLength(100).WithMessage("Time of day cannot exceed 100 characters.");

        RuleFor(x => x.MealInstructions)
            .MaximumLength(255).WithMessage("Meal instructions cannot exceed 255 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
    }
}