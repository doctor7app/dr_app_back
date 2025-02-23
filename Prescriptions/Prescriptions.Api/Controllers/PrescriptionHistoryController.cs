using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Prescriptions.Application.Interfaces;
using Prescriptions.Infrastructure.Implementation;

namespace Prescriptions.Api.Controllers
{
    [ApiController]
    [Route("api/prescriptions/{prescriptionId}/history")]
    public class PrescriptionHistoryController : ODataController
    {
        private readonly IPrescriptionHistoryService _prescriptionHistoryService;

        public PrescriptionHistoryController(IPrescriptionHistoryService prescriptionHistoryService)
        {
            _prescriptionHistoryService = prescriptionHistoryService;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> GetHistory([FromODataUri] Guid prescriptionId)
        {
            var prescription = await _prescriptionHistoryService.GetPrescriptionHistoryAsync(prescriptionId);
            return prescription != null ? Ok(prescription) : NotFound();
        }

        [HttpPost("{eventId}/revert")]
        public async Task<IActionResult> RevertToVersion(
            Guid prescriptionId,
            Guid eventId)
        {
            await _prescriptionHistoryService.RevertPrescriptionToVersionAsync(prescriptionId, eventId);
            return NoContent();
        }
    }
}
