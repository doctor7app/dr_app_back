using DocumentManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController(IDocumentService documentService, ILogger<DocumentController> logger) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument(
        IFormFile file
        , [FromForm] bool encrypt
        , [FromForm] string author
        , [FromForm] string tags
        , [FromForm] string description)
    {
        logger.LogInformation("Uploading document: {FileName} with encryption: {Encrypt}", file.FileName, encrypt);

        try
        {
            var result = await documentService.UploadDocumentAsync(file, encrypt, author, tags, description);
            logger.LogInformation("Document uploaded successfully: {FileName}", file.FileName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while uploading document: {FileName}", file.FileName);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        logger.LogInformation("Downloading document with ID: {DocumentId}", id);

        try
        {
            var (fileName, contentType, fileStream, metadata) = await documentService.GetDocumentAsync(id);
            logger.LogInformation("Document downloaded successfully: {DocumentId}", id);

            // Return the document with metadata
            return File(fileStream, contentType, fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while downloading document: {DocumentId}", id);
            return StatusCode(500, "Internal Server Error");
        }
    }
}
