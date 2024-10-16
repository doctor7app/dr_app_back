namespace DocumentManagement.Core.Models;

public class Metadata
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }  // Foreign key to Document

    public long FileSize { get; set; }    // Size of the file in bytes
    public string Author { get; set; }    // The uploader/owner of the document
    public string Tags { get; set; }      // Comma-separated tags or labels
    public DateTime UploadedAt { get; set; }  // Timestamp when the file was uploaded
    public string Description { get; set; }   // Optional description of the document
    public bool Encrypted { get; set; }   // Optional description of the document

    // Navigation property
    public Document Document { get; set; }
}
