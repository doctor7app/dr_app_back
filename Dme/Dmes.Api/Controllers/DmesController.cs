using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Threading.Tasks;
using System;
using Dme.Application.DTOs.Consultations;
using Dme.Application.DTOs.Dmes;
using Dme.Application.Interfaces;

namespace Dmes.Api.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [Route("api")]
    public class DmesController : ODataController
    {
        private readonly IDmeService _dmeService;
        private readonly IConsultationService _consultationService;

        public DmesController(IDmeService dmeService,IConsultationService consultationService)
        {
            _dmeService = dmeService;
            _consultationService = consultationService;
        }


        #region Dme Actions

        ///  <summary>
        ///  Get item with parameter passed in the query.
        ///  </summary>
        [HttpGet("Dmes({key})")]
        [HttpGet("Dmes/{key}")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] Guid key)
        {
            return Ok(await _dmeService.Get(key));
        }

        ///  <summary>
        ///  Get list  with parameter passed in the query.(Odata filtered)
        ///  </summary>
        [HttpGet("Dmes")]
        [HttpGet("Dmes/$count")]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All, MaxOrderByNodeCount = 10)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _dmeService.Get());
        }

        ///  <summary>
        ///  Add a list of entities to the database from the body of the api call.(Passed by as an array)
        ///  </summary>
        /// <param name="entity"></param>
        [HttpPost("Dmes")]
        public async Task<IActionResult> Post([FromBody] DmeCreateDto entity)
        {
            return Ok(await _dmeService.Create(entity));
        }

        [HttpPatch("Dmes({key})")]
        [HttpPatch("Dmes/{key}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] Delta<DmeUpdateDto> entity)
        {
            return Ok(await _dmeService.Update(key, entity));
        }

        ///  <summary>
        ///  Delete client from the database.
        ///  </summary>
        /// <param name="key"></param>
        [HttpDelete("Dmes({key})")]
        [HttpDelete("Dmes/{key}")]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            return Ok(await _dmeService.Delete(key));
        }

        #endregion

        #region Consultations Actions

        [EnableQuery]
        [HttpGet("Dmes({idDme})/Consultations({idConsultation})")]
        public async Task<IActionResult> GetConsultation(Guid idDme, Guid idConsultation)
        {
            return Ok(await _consultationService.GetConsultationForDme(idDme,idConsultation));
        }

        [EnableQuery]
        [HttpGet("Dmes({idDme})/Consultations")]
        public async Task<IActionResult> GetConsultation(Guid idDme)
        {
            return Ok(await _consultationService.GetAllConsultationForDme(idDme));
        }

        [HttpPost("Dmes({idDme})/Consultations")]
        public async Task<IActionResult> CreateConsultation([FromODataUri] Guid idDme, [FromBody] ConsultationsCreateDto entity)
        {
            return Ok(await _consultationService.CreateConsultationForDme(idDme, entity));
        }

        [HttpPatch("Dmes({idDme})/Consultations({id})")]
        public async Task<IActionResult> PatchConsultation([FromODataUri] Guid idDme, [FromODataUri] Guid id, [FromBody] Delta<ConsultationsUpdateDto> entity)
        {
            return Ok(await _consultationService.UpdateConsultationForDme(idDme, id, entity));
        }

        [HttpDelete("Dmes({idDme})/Consultations({id})")]
        public async Task<IActionResult> DeleteConsultation([FromODataUri] Guid idDme, [FromODataUri] Guid id)
        {
            return Ok(await _consultationService.DeleteConsultationForDme(idDme, id));
        }

        #endregion
    }
}
