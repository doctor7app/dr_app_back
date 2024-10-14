using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Interfaces
{
    public interface IDocumentService
    {
        Task<string> UploadDocumentAsync(IFormFile file, bool encrypt, string author, string tags, string description);
        Task<(string FileName, string ContentType, Stream FileStream, Metadata Metadata)> GetDocumentAsync(Guid id);
        Task DeleteDocumentAsync(Guid id);
    }
}
