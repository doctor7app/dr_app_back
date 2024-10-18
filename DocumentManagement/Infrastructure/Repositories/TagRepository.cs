using DocumentManagement.Core.Interfaces.Data;
using DocumentManagement.Core.Models;
using DocumentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DocumentManagement.Infrastructure.Repositories;

public class TagRepository(DocumentDbContext context) : ITagRepository
{

    // Retrieve all tags
    public async Task<IEnumerable<Tag>> GetTagsAsync()
    {
        return await context.Tags.ToListAsync();
    }

    // Retrieve a single tag by its name
    public async Task<Tag> GetTagByNameAsync(string tagName)
    {
        return await context.Tags
            .FirstOrDefaultAsync(t => t.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase));
    }

    // Retrieve multiple tags by their names
    public async Task<IEnumerable<Tag>> GetTagsByNamesAsync(IEnumerable<string> tagNames)
    {
        var normalizedNames = tagNames.Select(t => t.Trim().ToLower()).ToList();
        return await context.Tags
            .Where(t => normalizedNames.Contains(t.Name.ToLower()))
            .ToListAsync();
    }

    // Add new tags to the database
    public async Task AddTagsAsync(IEnumerable<Tag> tags)
    {
        context.Tags.AddRange(tags);
        await context.SaveChangesAsync();
    }

    // Check if a tag exists by name
    public async Task<bool> TagExistsAsync(string tagName)
    {
        return await context.Tags
            .AnyAsync(t => t.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase));
    }

    public async Task DeleteUnusedTagsAsync()
    {
        var unusedTags = await context.Tags
            .Where(t => !context.DocumentTags.Any(dt => dt.TagId == t.Id))
            .ToListAsync();

        if (unusedTags.Count != 0)
        {
            // Remove the unused tags from the database
            context.Tags.RemoveRange(unusedTags);
            await context.SaveChangesAsync();
        }
    }
}
