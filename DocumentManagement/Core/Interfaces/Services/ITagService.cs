using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Interfaces.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetTagsByNamesAsync(IEnumerable<string> tagNames);
    Task DeleteUnusedTagsAsync();
}
