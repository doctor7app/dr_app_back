﻿using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Interfaces.Data;

public interface IMetadataRepository
{
    Task<Metadata> GetByIdAsync(Guid id);
    Task<IEnumerable<Metadata>> GetAllAsync();
    Task AddAsync(Metadata metadata);
    Task UpdateAsync(Metadata metadata);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Metadata>> GetByAuthorAsync(string author);
    Task<IEnumerable<Metadata>> GetByServicesAsync(List<string> services);
    Task<IEnumerable<Metadata>> GetByAuthorAndServicesAsync(string author, List<string> services);
}
