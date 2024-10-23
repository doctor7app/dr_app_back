using Dme.Dtos.Diagnostics;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Services.Interfaces;

public interface IDiagnosticService
{
    Task<object> Get(Guid id, Guid idConsultation);
    Task<IEnumerable<DiagnosticsReadDto>> Get(Guid idConsultation);
    Task<object> Create(DiagnosticsCreateDto entity);
    Task<object> Update(Guid key, Delta<DiagnosticsUpdateDto> entity);
    Task<object> Delete(Guid id);
}