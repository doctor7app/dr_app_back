namespace DocumentManagement.Core.Models;

public class Document
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string ContentType { get; set; }
    public DateTime CreatedAt { get; set; }
    public Metadata Metadata { get; set; }
    public ICollection<DocumentTag> DocumentTags { get; set; } = [];
}
