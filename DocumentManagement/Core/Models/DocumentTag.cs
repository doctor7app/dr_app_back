namespace DocumentManagement.Core.Models;

public class DocumentTag
{
    public Guid DocumentId { get; set; }
    public Document Document { get; set; }

    public Guid TagId { get; set; }
    public Tag Tag { get; set; }
}
