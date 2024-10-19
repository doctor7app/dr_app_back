using DocumentManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Infrastructure.Data;

public class DocumentDbContext(DbContextOptions<DocumentDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Metadata> Metadatas { get; set; }
    public DbSet<DocumentTag> DocumentTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Document>().ToTable("Documents");
        modelBuilder.Entity<Metadata>().ToTable("Metadatas");
        modelBuilder.Entity<Tag>().ToTable("Tags");
        modelBuilder.Entity<DocumentTag>().ToTable("DocumentTags");

        // Configure the many-to-many relationship
        modelBuilder.Entity<DocumentTag>()
            .HasKey(dt => new { dt.DocumentId, dt.TagId });

        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Document)
            .WithMany(d => d.DocumentTags)
            .HasForeignKey(dt => dt.DocumentId);

        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Tag)
            .WithMany(t => t.DocumentTags)
            .HasForeignKey(dt => dt.TagId);


        // One-to-one relationship between Document and Metadata
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Metadata)
            .WithOne(m => m.Document)
            .HasForeignKey<Metadata>(m => m.DocumentId);
    }
}
