using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Interfaces.Services;

public interface IDocumentService
{
    Task<string> UploadDocumentAsync(IFormFile file, bool encrypt, string author, string service, List<string> tags, string description);
    Task<string> CopyDocumentAsync(Guid id, string author, string service, List<string> tags, string description);
    Task UpdateDocumentTagsAsync(Guid id, List<string> tagNames);
    Task<(string FileName, string ContentType, Stream FileStream, Metadata Metadata)> GetDocumentAsync(Guid id);
    Task<(Stream FileStream, string FileName)> GetDocumentsByAuthorAsZipAsync(string author);
    Task<(Stream FileStream, string FileName)> GetDocumentsByServicesAsZipAsync(List<string> services);
    Task<(Stream FileStream, string FileName)> GetDocumentsByTagsAsZipAsync(List<string> tags);
    Task<(Stream FileStream, string FileName)> GetByAuthorAndServicesAsZipAsync(string author, List<string> services);
    Task<(Stream FileStream, string FileName)> GetByAuthorAndTagsAsZipAsync(string author, List<string> tags);
    Task<(Stream FileStream, string FileName)> GetByAuthorAndServicesAndTagsAsZipAsync(string author, List<string> services, List<string> tags);
    Task DeleteDocumentAsync(Guid id);
    Task DeleteMultipleAsync(IEnumerable<Document> documents);
    Task DeleteDocumentsByTagsAsync(List<string> tags);
    Task DeleteDocumentsByServicesAsync(List<string> services);
    Task DeleteDocumentsByAuthorAsync(string author);
}
