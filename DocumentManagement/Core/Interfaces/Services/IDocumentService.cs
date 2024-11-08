using CSharpFunctionalExtensions;
using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Interfaces.Services;

public interface IDocumentService
{
    Task<Result<string>> UploadDocumentAsync(IFormFile file, bool encrypt, string author, string service, List<string> tags, string description);
    Task<Result<string>> CopyDocumentAsync(Guid id, string author, string service, List<string> tags, string description);
    Task<Result<bool>> UpdateDocumentTagsAsync(Guid id, List<string> tagNames);
    Task<Result<(string FileName, string ContentType, Stream FileStream, Metadata Metadata)>> GetDocumentAsync(Guid id);
    Task<Result<(Stream FileStream, string FileName)>> GetDocumentsByAuthorAsZipAsync(string author);
    Task<Result<(Stream FileStream, string FileName)>> GetDocumentsByServicesAsZipAsync(List<string> services);
    Task<Result<(Stream FileStream, string FileName)>> GetDocumentsByTagsAsZipAsync(List<string> tags);
    Task<Result<(Stream FileStream, string FileName)>> GetByAuthorAndServicesAsZipAsync(string author, List<string> services);
    Task<Result<(Stream FileStream, string FileName)>> GetByAuthorAndTagsAsZipAsync(string author, List<string> tags);
    Task<Result<(Stream FileStream, string FileName)>> GetByAuthorAndServicesAndTagsAsZipAsync(string author, List<string> services, List<string> tags);
    Task<Result<bool>> DeleteDocumentAsync(Guid id);
    Task<Result<bool>> DeleteMultipleAsync(IEnumerable<Document> documents);
    Task<Result<bool>> DeleteDocumentsByTagsAsync(List<string> tags);
    Task<Result<bool>> DeleteDocumentsByServicesAsync(List<string> services);
    Task<Result<bool>> DeleteDocumentsByAuthorAsync(string author);
}
