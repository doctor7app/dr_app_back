using FluentValidation;
using Prescriptions.Application.Dtos.Items;

namespace Prescriptions.Application.Validators;

public class PrescriptionItemUpdateValidator : AbstractValidator<PrescriptionItemUpdateDto>
{
    public PrescriptionItemUpdateValidator()
    {
        RuleFor(x => x.Dosage)
            .Matches(@"^\d+(\.\d+)?(mg|g|ml|L|UI)$").When(x => !string.IsNullOrEmpty(x.Dosage));
    }
}