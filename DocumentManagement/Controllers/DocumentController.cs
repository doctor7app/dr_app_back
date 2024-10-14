using DocumentManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(
            IFormFile file
            , [FromForm] bool encrypt
            , [FromForm] string author
            , [FromForm] string tags
            , [FromForm] string description)
        {
            _logger.LogInformation("Uploading document: {FileName} with encryption: {Encrypt}", file.FileName, encrypt);

            try
            {
                var result = await _documentService.UploadDocumentAsync(file, encrypt, author, tags, description);
                _logger.LogInformation("Document uploaded successfully: {FileName}", file.FileName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading document: {FileName}", file.FileName);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadDocument(Guid id)
        {
            _logger.LogInformation("Downloading document with ID: {DocumentId}", id);

            try
            {
                var (fileName, contentType, fileStream, metadata) = await _documentService.GetDocumentAsync(id);
                _logger.LogInformation("Document downloaded successfully: {DocumentId}", id);

                // Return the document with metadata
                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while downloading document: {DocumentId}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
