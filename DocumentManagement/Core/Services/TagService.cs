using DocumentManagement.Core.Interfaces.Data;
using DocumentManagement.Core.Interfaces.Services;
using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Services;

public class TagService(ITagRepository tagRepository) : ITagService
{
    public async Task<IEnumerable<Tag>> GetTagsByNamesAsync(IEnumerable<string> tagNames)
    {
        return await tagRepository.GetTagsByNamesAsync(tagNames);
    }
    public async Task DeleteUnusedTagsAsync()
    {
        await tagRepository.DeleteUnusedTagsAsync();
    }
}
