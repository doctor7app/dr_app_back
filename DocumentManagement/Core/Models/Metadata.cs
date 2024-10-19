namespace DocumentManagement.Core.Models;

public class Metadata
{
    public Guid Id { get; set; }
    public long FileSize { get; set; }    // Size of the file in bytes
    public string Author { get; set; }    // The uploader/owner of the document
    public string Service { get; set; }    // The concerned service
    public DateTime UploadedAt { get; set; }  // Timestamp when the file was uploaded
    public string Description { get; set; }   // Optional description of the document
    public bool Encrypted { get; set; }   // Optional description of the document
    public Guid DocumentId { get; set; }  // Foreign key to Document
    public Document Document { get; set; } // Navigation property
}
