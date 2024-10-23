using Dme.Dtos.Treatments;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Services.Interfaces;

public interface ITreatmentService
{
    Task<object> Get(Guid id, Guid idConsultation);
    Task<IEnumerable<TreatmentsReadDto>> Get(Guid idConsultation);
    Task<object> Create(TreatmentsCreateDto entity);
    Task<object> Update(Guid key, Delta<TreatmentsUpdateDto> entity);
    Task<object> Delete(Guid id);
}