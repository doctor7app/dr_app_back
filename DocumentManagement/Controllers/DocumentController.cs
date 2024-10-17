using DocumentManagement.Core.Interfaces.Services;
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
        , [FromForm] string service
        , [FromForm] List<string> tags
        , [FromForm] string description
        )
    {
        logger.LogInformation("Uploading document: {FileName} with encryption: {Encrypt}", file.FileName, encrypt);


        try
        {
            var result = await documentService.UploadDocumentAsync(file, encrypt, author, service, tags, description);
            logger.LogInformation("Document uploaded successfully: {FileName}", file.FileName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while uploading document: {FileName}", file.FileName);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut("{id}/tags")]
    public async Task<IActionResult> UpdateDocumentTags(Guid id, [FromBody] List<string> tagNames)
    {
        try
        {
            await documentService.UpdateDocumentTagsAsync(id, tagNames);
            return Ok("Document tags updated successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
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

    [HttpGet("GetDocumentsByServicesAsZip")]
    public async Task<IActionResult> GetDocumentsByServicesAsZip([FromQuery] List<string> services)
    {
        try
        {
            var (zipStream, fileName) = await documentService.GetDocumentsByServicesAsZipAsync(services);

            if (zipStream == null)
            {
                logger.LogInformation("No documents found for the given services: {names}", services.Aggregate((a, b) => a + ", " + b));
                return NotFound("No documents found for the given services.");
            }
            // Step 6: Return the ZIP file as a downloadable response using the stream
            logger.LogInformation("Documents downloaded successfully for the given services: {names}", services.Aggregate((a, b) => a + ", " + b));
            return File(zipStream, "application/zip", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while downloading documents for the given services: {names}", services.Aggregate((a, b) => a + ", " + b));
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("GetDocumentsByTagsAsZip")]
    public async Task<IActionResult> GetDocumentsByTagsAsZip([FromQuery] List<string> tags)
    {
        try
        {
            var (zipStream, fileName) = await documentService.GetDocumentsByTagsAsZipAsync(tags);

            if (zipStream == null)
            {
                logger.LogInformation("No documents found for the given tags: {names}", tags.Aggregate((a, b) => a + ", " + b));
                return NotFound("No documents found for the given tags.");
            }
            // Step 6: Return the ZIP file as a downloadable response using the stream
            logger.LogInformation("Documents downloaded successfully for the given tags: {names}", tags.Aggregate((a, b) => a + ", " + b));
            return File(zipStream, "application/zip", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while downloading documents for the given tags: {names}", tags.Aggregate((a, b) => a + ", " + b));
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("GetDocumentsByAuthorAsZip")]
    public async Task<IActionResult> GetDocumentsByAuthorAsZip([FromQuery] string author)
    {
        try
        {
            var (zipStream, fileName) = await documentService.GetDocumentsByAuthorAsZipAsync(author);

            if (zipStream == null)
            {
                logger.LogInformation("No documents found for the given author: {name}", author);
                return NotFound("No documents found for the given author.");
            }
            // Step 6: Return the ZIP file as a downloadable response using the stream
            logger.LogInformation("Documents downloaded successfully for the given author: {name}", author);
            return File(zipStream, "application/zip", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while downloading documents for the given author: {name}", author);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        try
        {
            await documentService.DeleteDocumentAsync(id);
            return Ok("Document deleted successfully.");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("DeleteDocumentsByServices")]
    public async Task<IActionResult> DeleteDocumentsByServices([FromBody] List<string> services)
    {
        try
        {
            await documentService.DeleteDocumentsByServicesAsync(services);
            return Ok("Documents deleted successfully.");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("DeleteDocumentsByTags")]
    public async Task<IActionResult> DeleteDocumentsByTags([FromBody] List<string> tags)
    {
        try
        {
            await documentService.DeleteDocumentsByTagsAsync(tags);
            return Ok("Documents deleted successfully.");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("DeleteDocumentsByAuthor")]
    public async Task<IActionResult> DeleteDocumentsByAuthor([FromBody] string author)
    {
        try
        {
            await documentService.DeleteDocumentsByAuthorAsync(author);
            return Ok("Documents deleted successfully.");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

}
