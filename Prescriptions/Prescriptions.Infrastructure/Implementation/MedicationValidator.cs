using FluentValidation;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Interfaces;


namespace Prescriptions.Infrastructure.Implementation;

public class MedicationValidator : IMedicationValidator
{
    private readonly IValidator<PrescriptionItemCreateDto> _createValidator;
    private readonly IValidator<PrescriptionItemUpdateDto> _updateValidator;
    //private readonly IDrugCatalogService _drugCatalog; // Service externe (ex: catalogue de médicaments)

    public MedicationValidator(
        IValidator<PrescriptionItemCreateDto> createValidator,
        IValidator<PrescriptionItemUpdateDto> updateValidator)
        //IDrugCatalogService drugCatalog)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        //_drugCatalog = drugCatalog;
    }

    public async Task ValidateMedicationAsync(PrescriptionItemCreateDto dto)
    {
        // Validation FluentValidation
        var result = await _createValidator.ValidateAsync(dto);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        // Validation métier supplémentaire (ex: existence du médicament)
        //var exists = await _drugCatalog.ExistsAsync(dto.DrugName);
        //if (!exists)
        //    throw new ValidationException($"Le médicament {dto.DrugName} n'existe pas.");
    }

    public async Task ValidateMedicationUpdateAsync(PrescriptionItemUpdateDto dto)
    {
        var result = await _updateValidator.ValidateAsync(dto);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}