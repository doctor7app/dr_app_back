using Dme.Dtos.Dmes;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Services.Interfaces;

public interface IDmeService
{
    Task<object> Get(Guid id,Guid doctorId);
    Task<IEnumerable<DmeReadDto>> Get(Guid doctorId);
    Task<object> Create(DmeCreateDto entity);
    Task<object> Update(Guid idDme, Delta<DmeUpdateDto> entity);
    Task<object> Delete(Guid idDme);
}