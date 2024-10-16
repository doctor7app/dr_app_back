using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Interfaces.Data;

public interface IDocumentRepository
{
    Task<Document> GetByIdAsync(Guid id);
    Task<IEnumerable<Document>> GetAllAsync();
    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Document>> GetByAuthorAsync(string author);
    Task<IEnumerable<Document>> GetByServicesAsync(List<string> services);
    Task<IEnumerable<Document>> GetByTagsAsync(List<string> tags);
    Task<IEnumerable<Document>> GetByAuthorAndServicesAsync(string author, List<string> services);
    Task<IEnumerable<Document>> GetByAuthorAndTagsAsync(string author, List<string> tags);
    Task<IEnumerable<Document>> GetByAuthorAndServicesAndTagsAsync(string author, List<string> services, List<string> tags);
}
