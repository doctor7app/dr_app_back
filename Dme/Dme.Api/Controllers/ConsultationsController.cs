using Dme.Application.DTOs.Consultations;
using Dme.Application.DTOs.Diagnostics;
using Dme.Application.DTOs.Treatments;
using Dme.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Dme.Api.Controllers
{
    [Route("api/Consultations")]
    public class ConsultationsController : ODataController
    {
        private readonly IConsultationService _consultationService;
        private readonly ITreatmentService _treatmentService;
        private readonly IDiagnosticService _diagnosticService;

        public ConsultationsController(IConsultationService consultationService, ITreatmentService treatmentService,
            IDiagnosticService diagnosticService)
        {
            _consultationService = consultationService;
            _treatmentService = treatmentService;
            _diagnosticService = diagnosticService;
        }

        #region Consultations Actions

        [HttpGet("({key})")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] Guid key)
        {
            return Ok(await _consultationService.GetConsultationById(key));
        }

        [HttpGet]
        [EnableQuery(PageSize = 20)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _consultationService.GetAllConsultation());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ConsultationsCreateDto entity)
        {
            return Ok(await _consultationService.CreateConsultation(entity));
        }

        [HttpPatch("({key})")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] Delta<ConsultationsUpdateDto> entity)
        {
            return Ok(await _consultationService.UpdateConsultationById(key, entity));
        }

        [HttpDelete("({key})")]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            return Ok(await _consultationService.DeleteConsultationById(key));
        }

        #endregion

        #region Treatments Actions

        [HttpGet("({key})/Treatments({idTreatments})")]
        [EnableQuery]
        public async Task<IActionResult> GetTreatment([FromODataUri] Guid key,
            [FromODataUri] Guid idTreatments)
        {
            return Ok(await _treatmentService.GetTreatmentForConsultationById(key, idTreatments));
        }

        [HttpGet("({key})/Treatments")]
        [EnableQuery]
        public async Task<IActionResult> GetTreatment([FromODataUri] Guid key)
        {
            return Ok(await _treatmentService.GetAllTreatmentForConsultationById(key));
        }

        [HttpPost("({key})/Treatments")]
        public async Task<IActionResult> CreateTreatment([FromODataUri] Guid key,
            [FromBody] TreatmentsCreateDto entity)
        {
            return Ok(await _treatmentService.CreateTreatmentForConsultation(key, entity));
        }

        [HttpPatch("({key})/Treatments({idTreatments})")]
        public async Task<IActionResult> PatchTreatment([FromODataUri] Guid key,
            [FromODataUri] Guid idTreatments, [FromBody] Delta<TreatmentsUpdateDto> entity)
        {
            return Ok(await _treatmentService.UpdateTreatmentForConsultation(key, idTreatments, entity));
        }

        [HttpDelete("({key})/Treatments({idTreatments})")]
        public async Task<IActionResult> DeleteTreatment([FromODataUri] Guid key,
            [FromODataUri] Guid idTreatments)
        {
            return Ok(await _treatmentService.DeleteTreatmentForConsultation(key, idTreatments));
        }

        #endregion

        #region Diagnostics Actions

        [HttpGet("({key})/Diagnostics({idDiagnostics})")]
        [EnableQuery]
        public async Task<IActionResult> GetDiagnostic([FromODataUri] Guid key,
            [FromODataUri] Guid idDiagnostics)
        {
            return Ok(await _diagnosticService.GetDiagnosticForConsultation(key, idDiagnostics));
        }

        [HttpGet("({key})/Diagnostics")]
        [EnableQuery]
        public async Task<IActionResult> GetDiagnostic([FromODataUri] Guid key)
        {
            return Ok(await _diagnosticService.GetAllDiagnosticForConsultation(key));
        }

        [HttpPost("({key})/Diagnostics")]
        public async Task<IActionResult> CreateDiagnostic([FromODataUri] Guid key,
            [FromBody] DiagnosticsCreateDto entity)
        {
            return Ok(await _diagnosticService.CreateDiagnosticForConsultation(key, entity));
        }

        [HttpPatch("({key})/Diagnostics({idDiagnostics})")]
        public async Task<IActionResult> PatchDiagnostic([FromODataUri] Guid key,
            [FromODataUri] Guid idDiagnostics, [FromBody] Delta<DiagnosticsUpdateDto> entity)
        {
            return Ok(await _diagnosticService.UpdateDiagnosticForConsultation(key, idDiagnostics, entity));
        }

        [HttpDelete("({key})/Diagnostics({idDiagnostics})")]
        public async Task<IActionResult> DeleteDiagnostic([FromODataUri] Guid key,
            [FromODataUri] Guid idDiagnostics)
        {
            return Ok(await _diagnosticService.DeleteDiagnosticForConsultation(key, idDiagnostics));
        }

        #endregion
    }
}