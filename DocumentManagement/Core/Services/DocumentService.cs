using DocumentManagement.Core.Interfaces;
using DocumentManagement.Core.Models;
using DocumentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace DocumentManagement.Core.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly DocumentDbContext _dbContext;
        private readonly IStorageService _storageService;

        private static readonly Counter UploadedDocuments = Metrics.CreateCounter("uploaded_documents_total", "Total number of uploaded documents");
        private static readonly Counter DownloadedDocuments = Metrics.CreateCounter("downloaded_documents_total", "Total number of downloaded documents");
        private static readonly Counter DeletedDocuments = Metrics.CreateCounter("deleted_documents_total", "Total number of deleted documents");

        public DocumentService(DocumentDbContext dbContext, IStorageService storageService)
        {
            _dbContext = dbContext;
            _storageService = storageService;
        }

        public async Task<string> UploadDocumentAsync(IFormFile file, bool encrypt, string author, string tags, string description)
        {
            // Save file to local storage
            var filePath = await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName, encrypt);

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
                Description = description
            };

            // Save to the database
            _dbContext.Documents.Add(document);
            _dbContext.Metadatas.Add(metadata);
            await _dbContext.SaveChangesAsync();

            // Increment Prometheus counter
            UploadedDocuments.Inc();

            return document.Id.ToString();
        }

        public async Task<(string FileName, string ContentType, Stream FileStream, Metadata Metadata)> GetDocumentAsync(Guid id)
        {
            var document = await _dbContext.Documents
                .Include(d => d.Metadata)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
                throw new FileNotFoundException("Document not found");

            var fileStream = await _storageService.GetFileAsync(document.FilePath);

            // Increment Prometheus counter
            DownloadedDocuments.Inc();

            return (document.FileName, document.ContentType, fileStream, document.Metadata);
        }

        public async Task DeleteDocumentAsync(Guid id)
        {
            var document = await _dbContext.Documents.Include(d => d.Metadata).FirstOrDefaultAsync(d => d.Id == id);
            if (document == null)
                throw new FileNotFoundException("Document not found");

            await _storageService.DeleteFileAsync(document.FilePath);
            _dbContext.Documents.Remove(document);
            _dbContext.Metadatas.Remove(document.Metadata);
            await _dbContext.SaveChangesAsync();

            // Increment Prometheus counter
            DeletedDocuments.Inc();
        }
    }
}
