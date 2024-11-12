using Microsoft.AspNetCore.OData.Deltas;
using Patients.Application.DTOs.Patient;

namespace Patients.Application.Interfaces;

public interface IPatientService
{
    Task<object> Get(Guid id);
    Task<IEnumerable<PatientDto>> Get();
    Task<object> Create(PatientCreateDto entity);
    Task<object> Update(Guid key, Delta<PatientUpdateDto> entity);
    Task<object> Delete(Guid id);
}