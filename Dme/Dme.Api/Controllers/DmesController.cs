using Dme.Application.DTOs.Consultations;
using Dme.Application.DTOs.Dmes;
using Dme.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Dme.Api.Controllers
{
    [Route("api/Dmes")]
    public class DmesController : ODataController
    {
        private readonly IDmeService _dmeService;
        private readonly IConsultationService _consultationService;

        public DmesController(IDmeService dmeService, IConsultationService consultationService)
        {
            _dmeService = dmeService;
            _consultationService = consultationService;
        }

        #region Dme Actions
        
        [HttpGet("{key}")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] Guid key)
        {
            return Ok(await _dmeService.Get(key));
        }
        
        [HttpGet]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All, MaxOrderByNodeCount = 10)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _dmeService.Get());
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DmeCreateDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }
            return Ok(await _dmeService.Create(entity));
        }

        [HttpPatch("{key}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] DmePatchDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }
            return Ok(await _dmeService.Patch(key, entity));
        }
        
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            return Ok(await _dmeService.Delete(key));
        }

        #endregion

        #region Consultations Actions

        [EnableQuery]
        [HttpGet("{key}/Consultations/{idConsultation}")]
        public async Task<IActionResult> GetConsultation(Guid key, Guid idConsultation)
        {
            return Ok(await _consultationService.GetConsultationForDme(key, idConsultation));
        }

        [EnableQuery]
        [HttpGet("{key}/Consultations")]
        public async Task<IActionResult> GetConsultation(Guid key)
        {
            return Ok(await _consultationService.GetAllConsultationForDme(key));
        }

        [HttpPost("{key}/Consultations")]
        public async Task<IActionResult> CreateConsultation([FromODataUri] Guid key, [FromBody] ConsultationsCreateDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }
            return Ok(await _consultationService.CreateConsultationForDme(key, entity));
        }

        [HttpPatch("{key}/Consultations/{idConsultation}")]
        public async Task<IActionResult> PatchConsultation([FromODataUri] Guid key, 
            [FromODataUri] Guid idConsultation, 
            [FromBody] ConsultationsPatchDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }
            return Ok(await _consultationService.PatchConsultationForDme(key, idConsultation, entity));
        }

        [HttpDelete("{key}/Consultations/{idConsultation}")]
        public async Task<IActionResult> DeleteConsultation([FromODataUri] Guid key, [FromODataUri] Guid idConsultation)
        {
            return Ok(await _consultationService.DeleteConsultationForDme(key, idConsultation));
        }

        #endregion
    }
}
