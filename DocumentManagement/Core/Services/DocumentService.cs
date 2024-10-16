using DocumentManagement.Core.Interfaces;
using DocumentManagement.Core.Models;
using DocumentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace DocumentManagement.Core.Services;

public class DocumentService(DocumentDbContext dbContext, IStorageService storageService) : IDocumentService
{
    private static readonly Counter UploadedDocuments = Metrics.CreateCounter("uploaded_documents_total", "Total number of uploaded documents");
    private static readonly Counter DownloadedDocuments = Metrics.CreateCounter("downloaded_documents_total", "Total number of downloaded documents");
    private static readonly Counter DeletedDocuments = Metrics.CreateCounter("deleted_documents_total", "Total number of deleted documents");

    public async Task<string> UploadDocumentAsync(IFormFile file, bool encrypt, string author, string tags, string description)
    {
        // Save file to local storage
        var filePath = await storageService.SaveFileAsync(file.OpenReadStream(), file.FileName, encrypt);

        // Create the Document and Metadata objects
        var document = new Document
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            FilePath = filePath,
            ContentType = file.ContentType,
            CreatedAt = DateTime.UtcNow
        };

        var metadata = new Metadata
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            FileSize = file.Length,
            Author = author,
            Tags = tags,
            UploadedAt = DateTime.UtcNow,
            Description = description,
            Encrypted = encrypt
        };

        // Save to the database
        dbContext.Documents.Add(document);
        dbContext.Metadatas.Add(metadata);
        await dbContext.SaveChangesAsync();

        // Increment Prometheus counter
        UploadedDocuments.Inc();

        return document.Id.ToString();
    }

    public async Task<(string FileName, string ContentType, Stream FileStream, Metadata Metadata)> GetDocumentAsync(Guid id)
    {
        var document = await dbContext.Documents
            .Include(d => d.Metadata)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new FileNotFoundException("Document not found");
        var fileStream = await storageService.GetFileAsync(document.FilePath, document.Metadata.Encrypted);

        // Increment Prometheus counter
        DownloadedDocuments.Inc();

        return (document.FileName, document.ContentType, fileStream, document.Metadata);
    }

    public async Task DeleteDocumentAsync(Guid id)
    {
        var document = await dbContext.Documents.Include(d => d.Metadata).FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new FileNotFoundException("Document not found");
        await storageService.DeleteFileAsync(document.FilePath);
        dbContext.Documents.Remove(document);
        dbContext.Metadatas.Remove(document.Metadata);
        await dbContext.SaveChangesAsync();

        // Increment Prometheus counter
        DeletedDocuments.Inc();
    }
}
