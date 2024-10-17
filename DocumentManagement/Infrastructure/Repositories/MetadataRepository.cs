using DocumentManagement.Core.Interfaces.Data;
using DocumentManagement.Core.Models;
using DocumentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Infrastructure.Repositories;

public class MetadataRepository(DocumentDbContext context) : IMetadataRepository
{
    public async Task<Metadata> GetByIdAsync(Guid id)
    {
        return await context.Metadatas
            .Include(m => m.Document)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Metadata>> GetAllAsync()
    {
        return await context.Metadatas
            .Include(m => m.Document)
            .ToListAsync();
    }

    public async Task AddAsync(Metadata metadata)
    {
        await context.Metadatas.AddAsync(metadata);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Metadata metadata)
    {
        context.Metadatas.Update(metadata);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var metadata = await GetByIdAsync(id);
        if (metadata != null)
        {
            context.Metadatas.Remove(metadata);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Metadata>> GetByAuthorAsync(string author)
    {
        return await context.Metadatas
            .Where(m => m.Author == author)
            .Include(m => m.Document)
            .ToListAsync();
    }

    public async Task<IEnumerable<Metadata>> GetByServicesAsync(List<string> services)
    {
        return await context.Metadatas
            .Where(m => services.Contains(m.Service))
            .Include(m => m.Document)
            .ToListAsync();
    }

    public async Task<IEnumerable<Metadata>> GetByAuthorAndServicesAsync(string author, List<string> services)
    {
        return await context.Metadatas
            .Where(m => m.Author == author)
            .Where(m => services.Contains(m.Service))
            .Include(m => m.Document)
            .ToListAsync();
    }

}
