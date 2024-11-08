using CSharpFunctionalExtensions;
using DocumentManagement.Core.Interfaces.Data;
using DocumentManagement.Core.Interfaces.Services;
using DocumentManagement.Core.Models;
using DocumentManagement.Core.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog.Context;
using System.IO.Compression;

namespace DocumentManagement.Core.Services;

public class DocumentService(IStorageService storageService, IDocumentRepository documentRepository, ITagService tagService, ILogger<DocumentService> logger) : IDocumentService
{
    private static readonly Counter UploadedDocuments = Metrics.CreateCounter("uploaded_documents_total", "Total number of uploaded documents");
    private static readonly Counter CopiedDocuments = Metrics.CreateCounter("copied_documents_total", "Total number of copied documents");
    private static readonly Counter DownloadedDocuments = Metrics.CreateCounter("downloaded_documents_total", "Total number of downloaded documents");
    private static readonly Counter DownloadedZipDocuments = Metrics.CreateCounter("downloaded_zip_documents_total", "Total number of downloaded zip documents");
    private static readonly Counter EditDocumentsTags = Metrics.CreateCounter("edit_documents_tags_total", "Total number of documents tags edit");
    private static readonly Counter DeletedDocuments = Metrics.CreateCounter("deleted_documents_total", "Total number of deleted documents");

    public async Task<Result<string>> UploadDocumentAsync(IFormFile file, bool encrypt, string author, string service, List<string> tags, string description)
    {
        // Save file to local storage
        var result = await storageService.SaveFileAsync(file.OpenReadStream(), file.FileName, encrypt);

        if (result.IsFailure)
        {
            return result;
        }
        try
        {
            // Create the Document and Metadata objects
            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FilePath = result.Value,
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
                UploadedAt = DateTime.UtcNow,
                Description = description,
                Encrypted = encrypt
            };
            document.Metadata = metadata;

            // handle tags
            var existingTagsResult = await tagService.GetTagsByNamesAsync(tags);
            if (existingTagsResult.IsFailure)
            {
                return Result.Failure<string>(existingTagsResult.Error);
            }
            var newTags = tags.Except(existingTagsResult.Value.Select(t => t.Name))
                .Select(t => new Tag { Name = t })
                .ToList();
            foreach (var tag in (List<Tag>)[.. existingTagsResult.Value, .. newTags])
            {
                document.DocumentTags.Add(new DocumentTag { Document = document, Tag = tag });
            }

            // Save to the database
            await documentRepository.AddAsync(document);

            // Increment Prometheus counter
            UploadedDocuments.Inc();

            return document.Id.ToString();
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<string>("Error accured while Uploading Document");
        }

    }

    public async Task<Result<string>> CopyDocumentAsync(Guid id, string author, string service, List<string> tags, string description)
    {
        try
        {
            var originalDocument = await documentRepository.GetByIdAsync(id) ?? throw new FileNotFoundException("Document not found");

            // Copy file in local storage
            var filePathResult = await storageService.CopyFileAsync(originalDocument.FilePath, originalDocument.FileName);
            if (filePathResult.IsFailure)
            {
                return filePathResult;
            }

            // Create the Document and Metadata objects
            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = originalDocument.FileName,
                FilePath = filePathResult.Value,
                ContentType = originalDocument.ContentType,
                CreatedAt = DateTime.UtcNow
            };

            var metadata = new Metadata
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                FileSize = originalDocument.Metadata.FileSize,
                Author = author,
                Service = service,
                UploadedAt = DateTime.UtcNow,
                Description = description,
                Encrypted = originalDocument.Metadata.Encrypted
            };
            document.Metadata = metadata;

            // handle tags
            var existingTagsResult = await tagService.GetTagsByNamesAsync(tags);
            if (existingTagsResult.IsFailure)
            {
                return Result.Failure<string>(existingTagsResult.Error);
            }
            var newTags = tags.Except(existingTagsResult.Value.Select(t => t.Name))
                .Select(t => new Tag { Name = t })
                .ToList();
            foreach (var tag in (List<Tag>)[.. existingTagsResult.Value, .. newTags])
            {
                document.DocumentTags.Add(new DocumentTag { Document = document, Tag = tag });
            }

            // Save to the database
            await documentRepository.AddAsync(document);

            // Increment Prometheus counter
            CopiedDocuments.Inc();

            return document.Id.ToString();
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<string>("Error accured while Getting Document");
        }

    }

    public async Task<Result<bool>> UpdateDocumentTagsAsync(Guid documentId, List<string> tagNames)
    {
        try
        {
            await documentRepository.UpdateDocumentTagsAsync(documentId, tagNames);
            await tagService.DeleteUnusedTagsAsync();
            EditDocumentsTags.Inc();
            return true;
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<bool>("Error accured while updating Document tags");
        }
    }

    public async Task<Result<(string FileName, string ContentType, Stream FileStream, Metadata Metadata)>> GetDocumentAsync(Guid id)
    {
        try
        {
            var document = await documentRepository.GetByIdAsync(id) ?? throw new FileNotFoundException("Document not found");
            var result = await storageService.GetFileAsync(document.FilePath, document.Metadata.Encrypted);
            if (result.IsFailure)
            {
                return Result.Failure<(string FileName, string ContentType, Stream FileStream, Metadata Metadata)>(result.Error);
            }

            // Increment Prometheus counter
            DownloadedDocuments.Inc();

            return (document.FileName, document.ContentType, result.Value, document.Metadata);
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<(string FileName, string ContentType, Stream FileStream, Metadata Metadata)>("Error accured while Getting Document");
        }
    }

    public async Task<Result<bool>> DeleteDocumentAsync(Guid id)
    {
        try
        {
            var document = await documentRepository.GetByIdAsync(id) ?? throw new FileNotFoundException("Document not found");
            var result = storageService.DeleteFileAsync(document.FilePath);
            if (result.IsFailure)
            {
                return Result.Failure<bool>(result.Error);
            }
            await documentRepository.DeleteAsync(document.Id);
            await tagService.DeleteUnusedTagsAsync();
            DeletedDocuments.Inc();
            return result;
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<bool>("Error accured while Deleting Document");
        }

    }

    public async Task<Result<bool>> DeleteDocumentsByTagsAsync(List<string> tags)
    {
        return await DeleteMultipleAsync(await documentRepository.GetByTagsAsync(tags));

    }

    public async Task<Result<bool>> DeleteDocumentsByServicesAsync(List<string> services)
    {
        return await DeleteMultipleAsync(await documentRepository.GetByServicesAsync(services));

    }

    public async Task<Result<bool>> DeleteDocumentsByAuthorAsync(string author)
    {
        return await DeleteMultipleAsync(await documentRepository.GetByAuthorAsync(author));

    }

    public async Task<Result<bool>> DeleteMultipleAsync(IEnumerable<Document> documents)
    {
        try
        {
            foreach (var document in documents)
            {
                var deleteFileResult = storageService.DeleteFileAsync(document.FilePath);
                if (deleteFileResult.IsFailure)
                {
                    return deleteFileResult;
                }
            }
            await documentRepository.DeleteMultipleAsync(documents);
            var deleteUnusedTagsResult = await tagService.DeleteUnusedTagsAsync();
            return deleteUnusedTagsResult;
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<bool>("Error accured while Deleting multiple Documents");
        }

    }

    public async Task<Result<(Stream FileStream, string FileName)>> GetDocumentsByServicesAsZipAsync(List<string> services)
    {
        try
        {
            var documents = await documentRepository.GetByServicesAsync(services);

            return (await GetZipStreamAsync(documents), GetFileName(services));
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<(Stream FileStream, string FileName)>("Error accured while Getting Documents By Services As Zip");
        }

    }
    public async Task<Result<(Stream FileStream, string FileName)>> GetDocumentsByTagsAsZipAsync(List<string> tags)
    {
        try
        {
            var documents = await documentRepository.GetByTagsAsync(tags);

            return (await GetZipStreamAsync(documents), GetFileName(tags));
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<(Stream FileStream, string FileName)>("Error accured while Getting Documents By Tags As Zip");
        }
    }
    public async Task<Result<(Stream FileStream, string FileName)>> GetDocumentsByAuthorAsZipAsync(string author)
    {
        try
        {
            var documents = await documentRepository.GetByAuthorAsync(author);

            return (await GetZipStreamAsync(documents), GetFileName([author]));
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<(Stream FileStream, string FileName)>("Error accured while Getting Documents By Author As Zip");
        }
    }

    public async Task<Result<(Stream FileStream, string FileName)>> GetByAuthorAndServicesAsZipAsync(string author, List<string> services)
    {
        try
        {
            var documents = await documentRepository.GetByAuthorAndServicesAsync(author, services);

            return (await GetZipStreamAsync(documents), GetFileName([author, .. services]));
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<(Stream FileStream, string FileName)>("Error accured while Getting Documents By Services As Zip");
        }
    }

    public async Task<Result<(Stream FileStream, string FileName)>> GetByAuthorAndTagsAsZipAsync(string author, List<string> tags)
    {
        try
        {
            var documents = await documentRepository.GetByAuthorAndTagsAsync(author, tags);

            return (await GetZipStreamAsync(documents), GetFileName([author, .. tags]));
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<(Stream FileStream, string FileName)>("Error accured while Getting Documents By Tags As Zip");
        }
    }

    public async Task<Result<(Stream FileStream, string FileName)>> GetByAuthorAndServicesAndTagsAsZipAsync(string author, List<string> services, List<string> tags)
    {
        try
        {
            var documents = await documentRepository.GetByAuthorAndServicesAndTagsAsync(author, services, tags);

            return (await GetZipStreamAsync(documents), GetFileName([author, .. services, .. tags]));
        }
        catch (Exception e)
        {
            using (LogContext.PushProperty("Error", e.StackTrace, true))
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
            return Result.Failure<(Stream FileStream, string FileName)>("Error accured while Getting Documents By Services And Tags As Zip");
        }
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
                if (fileNameTracker.TryGetValue(fileName, out int value))
                {
                    // Increment the index for this file name and create a new one
                    fileNameTracker[fileName] = ++value;
                    var index = value;
                    // Create a new file name with the index (e.g., FileName(1).pdf)
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}({index}){Path.GetExtension(fileName)}";
                }
                else
                {
                    fileNameTracker[fileName] = 0;
                }
                var fileStreamResult = await storageService.GetFileAsync(document.FilePath, document.Metadata.Encrypted);
                if (fileStreamResult.IsFailure)
                {
                    throw new Exception(fileStreamResult.Error);
                }
                // Create a new zip entry for each document
                var zipEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                using var zipStream = zipEntry.Open();
                await fileStreamResult.Value.CopyToAsync(zipStream);
                fileStreamResult.Value.Dispose();
            }
        }
        archive.Dispose();
        // Increment Prometheus counter
        DownloadedZipDocuments.Inc();
        memoryStream.Position = 0;
        return memoryStream;
    }

    private static string GetFileName(List<string> names)
    {
        return $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{string.Join("_", names)}_Documents.zip";
    }
}
