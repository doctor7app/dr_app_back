using DocumentManagement.Core.Interfaces.Data;
using DocumentManagement.Core.Interfaces.Services;
using DocumentManagement.Core.Models;
using DocumentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using System.IO.Compression;

namespace DocumentManagement.Core.Services;

public class DocumentService(IStorageService storageService, IDocumentRepository documentRepository) : IDocumentService
{
    private static readonly Counter UploadedDocuments = Metrics.CreateCounter("uploaded_documents_total", "Total number of uploaded documents");
    private static readonly Counter DownloadedDocuments = Metrics.CreateCounter("downloaded_documents_total", "Total number of downloaded documents");
    private static readonly Counter DeletedDocuments = Metrics.CreateCounter("deleted_documents_total", "Total number of deleted documents");

    public async Task<string> UploadDocumentAsync(IFormFile file, bool encrypt, string author, string service, string tags, string description)
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
            Service = service,
            Tags = tags,
            UploadedAt = DateTime.UtcNow,
            Description = description,
            Encrypted = encrypt
        };
        document.Metadata = metadata;

        // Save to the database
        await documentRepository.AddAsync(document);

        // Increment Prometheus counter
        UploadedDocuments.Inc();

        return document.Id.ToString();
    }

    public async Task<(string FileName, string ContentType, Stream FileStream, Metadata Metadata)> GetDocumentAsync(Guid id)
    {
        var document = await documentRepository.GetByIdAsync(id) ?? throw new FileNotFoundException("Document not found");
        var fileStream = await storageService.GetFileAsync(document.FilePath, document.Metadata.Encrypted);

        // Increment Prometheus counter
        DownloadedDocuments.Inc();

        return (document.FileName, document.ContentType, fileStream, document.Metadata);
    }

    public async Task DeleteDocumentAsync(Guid id)
    {
        var document = await documentRepository.GetByIdAsync(id) ?? throw new FileNotFoundException("Document not found");
        await storageService.DeleteFileAsync(document.FilePath);
        await documentRepository.DeleteAsync(document.Id);

        // Increment Prometheus counter
        DeletedDocuments.Inc();
    }

    public async Task<(Stream FileStream, string FileName)> GetDocumentsByServicesAsZipAsync(List<string> services)
    {
        var documents = await documentRepository.GetByServicesAsync(services);

        return (await GetZipStreamAsync(documents), GetFileName(services));
    }
    public async Task<(Stream FileStream, string FileName)> GetDocumentsByTagsAsZipAsync(List<string> tags)
    {
        var documents = await documentRepository.GetByTagsAsync(tags);

        return (await GetZipStreamAsync(documents), GetFileName(tags));
    }
    public async Task<(Stream FileStream, string FileName)> GetDocumentsByAuthorAsZipAsync(string author)
    {
        var documents = await documentRepository.GetByAuthorAsync(author);

        return (await GetZipStreamAsync(documents), GetFileName([author]));
    }

    public async Task<(Stream FileStream, string FileName)> GetByAuthorAndServicesAsZipAsync(string author, List<string> services)
    {
        var documents = await documentRepository.GetByAuthorAndServicesAsync(author, services);

        return (await GetZipStreamAsync(documents), GetFileName([author,.. services])); 
    }

    public async Task<(Stream FileStream, string FileName)> GetByAuthorAndTagsAsZipAsync(string author, List<string> tags)
    {
        var documents = await documentRepository.GetByAuthorAndTagsAsync(author, tags);

        return (await GetZipStreamAsync(documents), GetFileName([author,.. tags]));
    }

    public async Task<(Stream FileStream, string FileName)> GetByAuthorAndServicesAndTagsAsZipAsync(string author, List<string> services, List<string> tags)
    {
        var documents = await documentRepository.GetByAuthorAndServicesAndTagsAsync(author, services, tags);

        return (await GetZipStreamAsync(documents), GetFileName([author, .. services, .. tags]));
    }

    private async Task<Stream> GetZipStreamAsync(IEnumerable<Document> documents)
    {
        if (documents == null || !documents.Any())
            return null;

        // Step 1: Create a memory stream to hold the zip file
        var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
        var fileNameTracker = new Dictionary<string, int>();
        // Step 2: Loop through each document and add its file to the zip
        foreach (var document in documents)
        {
            if (File.Exists(document.FilePath))
            {
                var fileName = document.FileName;
                // Check if the file name already exists
                if (fileNameTracker.ContainsKey(fileName))
                {
                    // Increment the index for this file name and create a new one
                    fileNameTracker[fileName]++;
                    var index = fileNameTracker[fileName];
                    // Create a new file name with the index (e.g., FileName(1).pdf)
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}({index}){Path.GetExtension(fileName)}";
                }
                else
                {
                    fileNameTracker[fileName] = 0;
                }
                using var fileStream = await storageService.GetFileAsync(document.FilePath, document.Metadata.Encrypted);
                // Create a new zip entry for each document
                var zipEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                using var zipStream = zipEntry.Open();
                await fileStream.CopyToAsync(zipStream);
            }
        }
        archive.Dispose();
        memoryStream.Position = 0;
        return memoryStream;
    }

    private static string GetFileName(List<string> names)
    {
        return $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{string.Join("_", names)}_Documents.zip";
    }
}
