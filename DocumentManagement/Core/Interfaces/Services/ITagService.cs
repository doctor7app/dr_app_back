using CSharpFunctionalExtensions;
using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Interfaces.Services;

public interface ITagService
{
    Task<Result<IEnumerable<Tag>>> GetTagsByNamesAsync(IEnumerable<string> tagNames);
    Task<Result<bool>> DeleteUnusedTagsAsync();
}
