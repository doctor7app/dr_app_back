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
    [Route("api")]
    public class ConsultationsController(IConsultationService consultationService, ITreatmentService treatmentService, IDiagnosticService diagnosticService) : ODataController
    {

        #region Consultations Actions

        ///  <summary>
        ///  Get item with parameter passed in the query.
        ///  </summary>
        [HttpGet("Consultations({key})")]
        [HttpGet("Consultations/{key}")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] Guid key)
        {
            return Ok(await consultationService.GetConsultationById(key));
        }

        ///  <summary>
        ///  Get list  with parameter passed in the query.(Odata filtered)
        ///  </summary>
        [HttpGet("Consultations")]
        [HttpGet("Consultations/$count")]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All, MaxOrderByNodeCount = 10)]
        public async Task<IActionResult> Get()
        {
            return Ok(await consultationService.GetAllConsultation());
        }

        ///  <summary>
        ///  Add a list of entities to the database from the body of the api call.(Passed by as an array)
        ///  </summary>
        /// <param name="entity"></param>
        [HttpPost("Consultations")]
        public async Task<IActionResult> Post([FromBody] ConsultationsCreateDto entity)
        {
            return Ok(await consultationService.CreateConsultation(entity));
        }

        [HttpPatch("Consultations({key})")]
        [HttpPatch("Consultations/{key}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] Delta<ConsultationsUpdateDto> entity)
        {
            return Ok(await consultationService.UpdateConsultationById(key, entity));
        }

        ///  <summary>
        ///  Delete client from the database.
        ///  </summary>
        /// <param name="key"></param>
        [HttpDelete("Consultations({key})")]
        [HttpDelete("Consultations/{key}")]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            return Ok(await consultationService.DeleteConsultationById(key));
        }

        #endregion

        #region Treatments Actions

        [EnableQuery]
        [HttpGet("Consultations({idConsultation})/Treatments({idTreatments})")]
        public async Task<IActionResult> GetTreatment(Guid idConsultation, Guid idTreatments)
        {
            return Ok(await treatmentService.GetTreatmentForConsultationById(idConsultation, idTreatments));
        }

        [EnableQuery]
        [HttpGet("Consultations({idConsultation})/Treatments")]
        public async Task<IActionResult> GetTreatment(Guid idConsultation)
        {
            return Ok(await treatmentService.GetAllTreatmentForConsultationById(idConsultation));
        }

        [HttpPost("Consultations({idConsultation})/Treatments")]
        public async Task<IActionResult> CreateTreatment([FromODataUri] Guid idConsultation, [FromBody] TreatmentsCreateDto entity)
        {
            return Ok(await treatmentService.CreateTreatmentForConsultation(idConsultation, entity));
        }

        [HttpPatch("Consultations({idConsultation})/Treatments({idTreatments})")]
        public async Task<IActionResult> PatchTreatment([FromODataUri] Guid idConsultation, [FromODataUri] Guid idTreatments, [FromBody] Delta<TreatmentsUpdateDto> entity)
        {
            return Ok(await treatmentService.UpdateTreatmentForConsultation(idConsultation, idTreatments, entity));
        }

        [HttpDelete("Consultations({idConsultation})/Treatments({idTreatments})")]
        public async Task<IActionResult> DeleteTreatment([FromODataUri] Guid idConsultation, [FromODataUri] Guid idTreatments)
        {
            return Ok(await treatmentService.DeleteTreatmentForConsultation(idConsultation, idTreatments));
        }

        #endregion


        #region Diagnostics Actions

        [EnableQuery]
        [HttpGet("Consultations({idConsultation})/Diagnostics({idTreatments})")]
        public async Task<IActionResult> GetDiagnostic(Guid idConsultation, Guid idTreatments)
        {
            return Ok(await diagnosticService.GetDiagnosticForConsultation(idConsultation, idTreatments));
        }

        [EnableQuery]
        [HttpGet("Consultations({idConsultation})/Diagnostics")]
        public async Task<IActionResult> GetDiagnostic(Guid idConsultation)
        {
            return Ok(await diagnosticService.GetAllDiagnosticForConsultation(idConsultation));
        }

        [HttpPost("Consultations({idConsultation})/Diagnostics")]
        public async Task<IActionResult> CreateDiagnostic([FromODataUri] Guid idConsultation, [FromBody] DiagnosticsCreateDto entity)
        {
            return Ok(await diagnosticService.CreateDiagnosticForConsultation(idConsultation, entity));
        }

        [HttpPatch("Consultations({idConsultation})/Diagnostics({idDiagnostics})")]
        public async Task<IActionResult> PatchDiagnostic([FromODataUri] Guid idConsultation, [FromODataUri] Guid idDiagnostics, [FromBody] Delta<DiagnosticsUpdateDto> entity)
        {
            return Ok(await diagnosticService.UpdateDiagnosticForConsultation(idConsultation, idDiagnostics, entity));
        }

        [HttpDelete("Consultations({idConsultation})/Diagnostics({idDiagnostics})")]
        public async Task<IActionResult> DeleteDiagnostic([FromODataUri] Guid idConsultation, [FromODataUri] Guid idDiagnostics)
        {
            return Ok(await diagnosticService.DeleteDiagnosticForConsultation(idConsultation, idDiagnostics));
        }

        #endregion
    }
}
