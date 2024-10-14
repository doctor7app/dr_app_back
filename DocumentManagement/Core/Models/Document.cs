namespace DocumentManagement.Core.Models
{
    public class Document
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property for Metadata
        public Metadata Metadata { get; set; }
    }
}
