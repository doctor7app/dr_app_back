using Dme.Dtos.Consultations;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Services.Interfaces;

public interface IConsultationService
{
    Task<object> Get(Guid id, Guid idDme);
    Task<IEnumerable<ConsultationsReadDto>> Get(Guid idDme);
    Task<object> Create(ConsultationsCreateDto entity);
    Task<object> Update(Guid idConsultation, Delta<ConsultationsUpdateDto> entity);
    Task<object> Delete(Guid idConsultation);
}