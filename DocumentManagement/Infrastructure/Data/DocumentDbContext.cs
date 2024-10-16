using DocumentManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Infrastructure.Data;

public class DocumentDbContext(DbContextOptions<DocumentDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<Metadata> Metadatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Document>().ToTable("Documents");
        modelBuilder.Entity<Metadata>().ToTable("Metadatas");

        // One-to-one relationship between Document and Metadata
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Metadata)
            .WithOne(m => m.Document)
            .HasForeignKey<Metadata>(m => m.DocumentId);
    }
}
