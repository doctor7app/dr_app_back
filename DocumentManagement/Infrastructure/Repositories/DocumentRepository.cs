using DocumentManagement.Core.Interfaces.Data;
using DocumentManagement.Core.Models;
using DocumentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Infrastructure.Repositories;

public class DocumentRepository(DocumentDbContext context, ITagRepository tagRepository) : IDocumentRepository
{
    public async Task<Document> GetByIdAsync(Guid id)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Include(d => d.DocumentTags)
            .ThenInclude(dt => dt.Tag)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Include(d => d.DocumentTags)
            .ThenInclude(dt => dt.Tag)
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

    public async Task DeleteMultipleAsync(IEnumerable<Document> documents)
    {
        if (documents.Any())
        {
            context.Documents.RemoveRange(documents);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Document>> GetByAuthorAsync(string author)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d => d.Metadata.Author == author)
            .Include(d => d.DocumentTags)
            .ThenInclude(dt => dt.Tag)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByServicesAsync(List<string> services)
    {
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d => services.Contains(d.Metadata.Service))
            .Include(d => d.DocumentTags)
            .ThenInclude(dt => dt.Tag)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByTagsAsync(List<string> tags)
    {
        var normalizedTags = tags.Select(t => t.Trim().ToLower()).ToList();
        return await context.Documents
            .Where(d => d.DocumentTags.Any(dt => normalizedTags.Contains(dt.Tag.Name.ToLower())))
            .Include(d => d.Metadata)
            .Include(d => d.DocumentTags)
            .ThenInclude(dt => dt.Tag)
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
        var normalizedTags = tags.Select(t => t.Trim().ToLower()).ToList();
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d => d.Metadata.Author == author)
            .Where(d => d.DocumentTags.Any(dt => normalizedTags.Contains(dt.Tag.Name.ToLower())))
            .Include(d => d.DocumentTags)
            .ThenInclude(dt => dt.Tag)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByAuthorAndServicesAndTagsAsync(string author, List<string> services, List<string> tags)
    {
        var normalizedTags = tags.Select(t => t.Trim().ToLower()).ToList();
        return await context.Documents
            .Include(d => d.Metadata)
            .Where(d => d.Metadata.Author == author)
            .Where(d => services.Contains(d.Metadata.Service))
            .Where(d => d.DocumentTags.Any(dt => normalizedTags.Contains(dt.Tag.Name.ToLower())))
            .Include(d => d.DocumentTags)
            .ThenInclude(dt => dt.Tag)
            .ToListAsync();
    }

    public async Task UpdateDocumentTagsAsync(Guid id, List<string> tagNames)
    {
        var document = await GetByIdAsync(id) ?? throw new Exception("Document not found.");

        // Get the existing tags in the database
        var existingTags = await tagRepository.GetTagsByNamesAsync(tagNames);

        // Find new tags to add that don't exist yet
        var newTags = tagNames.Except(existingTags.Select(t => t.Name))
            .Select(t => new Tag { Name = t })
            .ToList();

        // Remove existing tags not in the new tag list
        var tagsToRemove = document.DocumentTags
            .Where(dt => !tagNames.Contains(dt.Tag.Name))
            .ToList();

        foreach (var tagToRemove in tagsToRemove)
        {
            document.DocumentTags.Remove(tagToRemove);
        }

        // Add new tags that are not already in the document
        foreach (var newTag in newTags)
        {
            document.DocumentTags.Add(
                new DocumentTag
                {
                    Document = document,
                    Tag = newTag
                });
        }

        await context.SaveChangesAsync();
    }

}
