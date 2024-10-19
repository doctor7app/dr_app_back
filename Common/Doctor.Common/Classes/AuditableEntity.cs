namespace Doctor.Common.Classes;

public abstract class AuditableEntity
{
    public DateTime Created { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime? LastModified { get; set; }

    public Guid LastModifiedById { get; set; }
}