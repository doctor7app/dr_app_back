using Dme.Application.DTOs.Consultations;
using Dme.Application.DTOs.Dmes;
using Dme.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Dme.Api.Controllers
{
    [Route("api")]
    public class DmesController(IDmeService dmeService, IConsultationService consultationService) : ODataController
    {


        #region Dme Actions

        ///  <summary>
        ///  Get item with parameter passed in the query.
        ///  </summary>
        [HttpGet("Dmes({key})")]
        [HttpGet("Dmes/{key}")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] Guid key)
        {
            return Ok(await dmeService.Get(key));
        }

        ///  <summary>
        ///  Get list  with parameter passed in the query.(Odata filtered)
        ///  </summary>
        [HttpGet("Dmes")]
        [HttpGet("Dmes/$count")]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All, MaxOrderByNodeCount = 10)]
        public async Task<IActionResult> Get()
        {
            return Ok(await dmeService.Get());
        }

        ///  <summary>
        ///  Add a list of entities to the database from the body of the api call.(Passed by as an array)
        ///  </summary>
        /// <param name="entity"></param>
        [HttpPost("Dmes")]
        public async Task<IActionResult> Post([FromBody] DmeCreateDto entity)
        {
            return Ok(await dmeService.Create(entity));
        }

        [HttpPatch("Dmes({key})")]
        [HttpPatch("Dmes/{key}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] Delta<DmeUpdateDto> entity)
        {
            return Ok(await dmeService.Update(key, entity));
        }

        ///  <summary>
        ///  Delete client from the database.
        ///  </summary>
        /// <param name="key"></param>
        [HttpDelete("Dmes({key})")]
        [HttpDelete("Dmes/{key}")]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            return Ok(await dmeService.Delete(key));
        }

        #endregion

        #region Consultations Actions

        [EnableQuery]
        [HttpGet("Dmes({idDme})/Consultations({idConsultation})")]
        public async Task<IActionResult> GetConsultation(Guid idDme, Guid idConsultation)
        {
            return Ok(await consultationService.GetConsultationForDme(idDme,idConsultation));
        }

        [EnableQuery]
        [HttpGet("Dmes({idDme})/Consultations")]
        public async Task<IActionResult> GetConsultation(Guid idDme)
        {
            return Ok(await consultationService.GetAllConsultationForDme(idDme));
        }

        [HttpPost("Dmes({idDme})/Consultations")]
        public async Task<IActionResult> CreateConsultation([FromODataUri] Guid idDme, [FromBody] ConsultationsCreateDto entity)
        {
            return Ok(await consultationService.CreateConsultationForDme(idDme, entity));
        }

        [HttpPatch("Dmes({idDme})/Consultations({id})")]
        public async Task<IActionResult> PatchConsultation([FromODataUri] Guid idDme, [FromODataUri] Guid id, [FromBody] Delta<ConsultationsUpdateDto> entity)
        {
            return Ok(await consultationService.UpdateConsultationForDme(idDme, id, entity));
        }

        [HttpDelete("Dmes({idDme})/Consultations({id})")]
        public async Task<IActionResult> DeleteConsultation([FromODataUri] Guid idDme, [FromODataUri] Guid id)
        {
            return Ok(await consultationService.DeleteConsultationForDme(idDme, id));
        }

        #endregion
    }
}
