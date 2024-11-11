﻿using Dme.Application.DTOs.Dmes;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Application.Interfaces;

public interface IDmeService
{
    Task<object> Get(Guid id);
    Task<IEnumerable<DmeReadDto>> Get();
    Task<object> Create(DmeCreateDto entity);
    Task<object> Update(Guid idDme, Delta<DmeUpdateDto> entity);
    Task<object> Delete(Guid idDme);
}