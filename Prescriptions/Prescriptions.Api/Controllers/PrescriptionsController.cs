using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Prescriptions.Application.Dtos.Events;
using Prescriptions.Application.Dtos.Prescriptions;
using Prescriptions.Application.Interfaces;

namespace Prescriptions.Api.Controllers
{
    [ApiController]
    [Route("api/prescriptions")]
    public class PrescriptionsController : ODataController
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionsController(
            IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 100)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var prescriptions = await _prescriptionService.GetAllPrescriptionAsync();
            return Ok(prescriptions.AsQueryable());
        }

        [EnableQuery]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromODataUri] Guid id)
        {
            var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
            return prescription != null ? Ok(prescription) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrescriptionCreateDto dto)
        {
            var result = await _prescriptionService.CreatePrescriptionAsync(dto);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid id,
            [FromBody] Delta<PrescriptionUpdateDto> patch)
        {
            var result = await _prescriptionService.UpdatePrescriptionAsync(id, patch);
            return result ? Ok(await _prescriptionService.GetPrescriptionByIdAsync(id)) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromODataUri] Guid id)
        {
            var result = await _prescriptionService.DeletePrescriptionAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(PrescriptionDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails([FromODataUri] Guid id)
        {
            var details = await _prescriptionService.GetPrescriptionDetailsAsync(id);
            return details != null ? Ok(details) : NotFound();
        }
    }
}
