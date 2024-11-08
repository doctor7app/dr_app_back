using CSharpFunctionalExtensions;
using DocumentManagement.Core.Interfaces.Data;
using DocumentManagement.Core.Interfaces.Services;
using DocumentManagement.Core.Models;
using Serilog.Context;

namespace DocumentManagement.Core.Services;

public class TagService(ITagRepository tagRepository, ILogger<TagService> logger) : ITagService
{
    public async Task<Result<IEnumerable<Tag>>> GetTagsByNamesAsync(IEnumerable<string> tagNames)
    {
        try
        {
            var tags = await tagRepository.GetTagsByNamesAsync(tagNames);
            return Result.Success(tags);
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<IEnumerable<Tag>>("Error accured while getting tags");
        }

    }
    public async Task<Result<bool>> DeleteUnusedTagsAsync()
    {
        try
        {
            await tagRepository.DeleteUnusedTagsAsync();
            return Result.Success(true);
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<bool>("Error accured while deleting unused tags");
        }
        
    }
}
