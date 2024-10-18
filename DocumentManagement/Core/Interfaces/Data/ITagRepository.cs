using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Interfaces.Data;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetTagsAsync();
    Task<Tag> GetTagByNameAsync(string tagName);
    Task<IEnumerable<Tag>> GetTagsByNamesAsync(IEnumerable<string> tagNames);
    Task AddTagsAsync(IEnumerable<Tag> tags);
    Task<bool> TagExistsAsync(string tagName);
    Task DeleteUnusedTagsAsync();
}
