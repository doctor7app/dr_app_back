using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Prescriptions.Application.Interfaces;

namespace Prescriptions.Api.Controllers
{
    [ApiController]
    [Route("api/history")]
    public class PrescriptionHistoryController : ODataController
    {
        private readonly IPrescriptionHistoryService _prescriptionHistoryService;

        public PrescriptionHistoryController(IPrescriptionHistoryService prescriptionHistoryService)
        {
            _prescriptionHistoryService = prescriptionHistoryService;
        }

        [EnableQuery]
        [HttpGet("prescriptions/{key}")]
        public async Task<IActionResult> GetPrescriptionHistory([FromODataUri] Guid key)
        {
            var prescription = await _prescriptionHistoryService.GetPrescriptionHistoryAsync(key);
            return prescription != null ? Ok(prescription) : NotFound();
        }

        [EnableQuery]
        [HttpGet("items/{itemId}")]
        public async Task<IActionResult> GetItemsHistory([FromODataUri] Guid key)
        {
            var prescription = await _prescriptionHistoryService.GetPrescriptionItemHistoryAsync(key);
            return prescription != null ? Ok(prescription) : NotFound();
        }
    }
}
