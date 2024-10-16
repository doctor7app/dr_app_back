using DocumentManagement.Core.Interfaces.Data;
using DocumentManagement.Core.Models;
using DocumentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Infrastructure.Repositories;

public class DocumentRepository(DocumentDbContext context) : IDocumentRepository
{
    public async Task<Document> GetByIdAsync(Guid id)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .ToListAsync();
    }

    public async Task AddAsync(Document document)
    {
        await context.Documents.AddAsync(document);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Document document)
    {
        context.Documents.Update(document);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var document = await GetByIdAsync(id);
        if (document != null)
        {
            context.Documents.Remove(document);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Document>> GetByAuthorAsync(string author)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d =>  d.Metadata.Author == author)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByServicesAsync(List<string> services)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d => services.Contains(d.Metadata.Service))
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByTagsAsync(List<string> tags)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d => tags.Any(tag => d.Metadata.Tags.Contains(tag)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByAuthorAndServicesAsync(string author, List<string> services)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d => d.Metadata.Author == author)
            .Where(d => services.Contains(d.Metadata.Service))
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByAuthorAndTagsAsync(string author, List<string> tags)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d => d.Metadata.Author == author)
            .Where(d => tags.Contains(d.Metadata.Service))
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByAuthorAndServicesAndTagsAsync(string author, List<string> services, List<string> tags)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d => d.Metadata.Author == author)
            .Where(d => services.Contains(d.Metadata.Service))
            .Where(d => tags.Contains(d.Metadata.Service))
            .ToListAsync();
    }
}
