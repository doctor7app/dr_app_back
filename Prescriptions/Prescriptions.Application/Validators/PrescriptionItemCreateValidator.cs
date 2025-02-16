using FluentValidation;
using Prescriptions.Application.Dtos.Items;

namespace Prescriptions.Application.Validators;

public class PrescriptionItemCreateValidator : AbstractValidator<PrescriptionItemCreateDto>
{
    public PrescriptionItemCreateValidator()
    {
        RuleFor(x => x.DrugName)
            .NotEmpty().WithMessage("Le nom du médicament est requis.")
            .MaximumLength(200);

        RuleFor(x => x.Dosage)
            .NotEmpty()
            .Matches(@"^\d+(\.\d+)?(mg|g|ml|L|UI)$").WithMessage("Format de dosage invalide.");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("La fréquence est requise.");
    }
}