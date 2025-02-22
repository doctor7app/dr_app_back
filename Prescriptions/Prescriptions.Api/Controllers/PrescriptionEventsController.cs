using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Prescriptions.Application.Dtos.Events;
using Prescriptions.Application.Interfaces;

namespace Prescriptions.Api.Controllers
{
    [ApiController]
    [Route("api/prescriptions/{prescriptionId}/events")]
    public class PrescriptionEventsController : ODataController
    {
        private readonly IPrescriptionEventService _prescriptionEventService;

        public PrescriptionEventsController(IPrescriptionEventService prescriptionEventService)
        {
            _prescriptionEventService = prescriptionEventService;
        }

        [HttpGet,EnableQuery]
        public async Task<ActionResult<IEnumerable<PrescriptionEventDto>>> Get(Guid prescriptionId)
        {
            var events = await _prescriptionEventService.GetEventsByPrescriptionAsync(prescriptionId);
            return Ok(events.AsQueryable());
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventDetails(
            [FromRoute] Guid prescriptionId,
            [FromRoute] Guid eventId)
        {
            
            var eventDetails = await _prescriptionEventService.GetEventDetailsAsync(eventId);
            return Ok(eventDetails);
        }
    }
}
