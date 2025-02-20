﻿using CSharpFunctionalExtensions;
using DocumentManagement.Core.Interfaces.Services;
using DocumentManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        var result = await documentService.UploadDocumentAsync(file, encrypt, author, service, tags, description);
        if (result.IsFailure)
        {
            return StatusCode(500, result.Error);
        }
        logger.LogInformation("Document uploaded successfully: {FileName}", file.FileName);
        return Ok(result.Value);
    }

    [HttpPost("copy")]
    public async Task<IActionResult> CopyDocument(
        [FromForm] Guid id
        , [FromForm] string author
        , [FromForm] string service
        , [FromForm] List<string> tags
        , [FromForm] string description
        )
    {
        logger.LogInformation("Copying document: {id}", id);

        var result = await documentService.CopyDocumentAsync(id, author, service, tags, description);
        if (result.IsFailure)
        {
            return StatusCode(404, result.Error);
        }
        logger.LogInformation("Document copied successfully: {id}", id);
        return Ok(result.Value);
    }

    [HttpPut("{id}/tags")]
    public async Task<IActionResult> UpdateDocumentTags(Guid id, [FromBody] List<string> tagNames)
    {
        var result = await documentService.UpdateDocumentTagsAsync(id, tagNames);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok("Document tags updated successfully.");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        logger.LogInformation("Downloading document with ID: {DocumentId}", id);
        var result = await documentService.GetDocumentAsync(id);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        var (fileName, contentType, fileStream, _) = result.Value;
        logger.LogInformation("Document downloaded successfully: {DocumentId}", id);

        // Return the document with metadata
        return File(fileStream, contentType, fileName);
    }

    [HttpGet("GetDocumentsByServicesAsZip")]
    public async Task<IActionResult> GetDocumentsByServicesAsZip([FromQuery] List<string> services)
    {
        var result = await documentService.GetDocumentsByServicesAsZipAsync(services);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        var (zipStream, fileName) = result.Value;
        if (zipStream == null)
        {
            logger.LogInformation("No documents found for the given services: {names}", services.Aggregate((a, b) => a + ", " + b));
            return NotFound("No documents found for the given services.");
        }
        // Step 6: Return the ZIP file as a downloadable response using the stream
        logger.LogInformation("Documents downloaded successfully for the given services: {names}", services.Aggregate((a, b) => a + ", " + b));
        return File(zipStream, "application/zip", fileName);
    }

    [HttpGet("GetDocumentsByTagsAsZip")]
    public async Task<IActionResult> GetDocumentsByTagsAsZip([FromQuery] List<string> tags)
    {
        var result = await documentService.GetDocumentsByTagsAsZipAsync(tags);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        var (zipStream, fileName) = result.Value;

        if (zipStream == null)
        {
            logger.LogInformation("No documents found for the given tags: {names}", tags.Aggregate((a, b) => a + ", " + b));
            return NotFound("No documents found for the given tags.");
        }
        // Step 6: Return the ZIP file as a downloadable response using the stream
        logger.LogInformation("Documents downloaded successfully for the given tags: {names}", tags.Aggregate((a, b) => a + ", " + b));
        return File(zipStream, "application/zip", fileName);
    }

    [HttpGet("GetDocumentsByAuthorAsZip")]
    public async Task<IActionResult> GetDocumentsByAuthorAsZip([FromQuery] string author)
    {
        var result = await documentService.GetDocumentsByAuthorAsZipAsync(author);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        var (zipStream, fileName) = result.Value;

        if (zipStream == null)
        {
            logger.LogInformation("No documents found for the given author: {name}", author);
            return NotFound("No documents found for the given author.");
        }
        // Step 6: Return the ZIP file as a downloadable response using the stream
        logger.LogInformation("Documents downloaded successfully for the given author: {name}", author);
        return File(zipStream, "application/zip", fileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        var result = await documentService.DeleteDocumentAsync(id);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok("Document deleted successfully.");
    }

    [HttpDelete("DeleteDocumentsByServices")]
    public async Task<IActionResult> DeleteDocumentsByServices([FromBody] List<string> services)
    {
        var result = await documentService.DeleteDocumentsByServicesAsync(services);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok("Documents deleted successfully.");
    }

    [HttpDelete("DeleteDocumentsByTags")]
    public async Task<IActionResult> DeleteDocumentsByTags([FromBody] List<string> tags)
    {
        var result = await documentService.DeleteDocumentsByTagsAsync(tags);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok("Documents deleted successfully.");
    }

    [HttpDelete("DeleteDocumentsByAuthor")]
    public async Task<IActionResult> DeleteDocumentsByAuthor([FromBody] string author)
    {
        var result = await documentService.DeleteDocumentsByAuthorAsync(author);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok("Documents deleted successfully.");
    }

}
