using Dme.Application.DTOs.Dmes;

namespace Dme.Application.Interfaces;

public interface IDmeService
{
    Task<object> Get(Guid id);
    Task<IEnumerable<DmeReadDto>> Get();
    Task<object> Create(DmeCreateDto entity);
    Task<object> Patch(Guid idDme, DmePatchDto entity);
    Task<object> Delete(Guid idDme);
}