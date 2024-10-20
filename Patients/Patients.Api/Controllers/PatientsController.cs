using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Patients.Dtos.Classes.Adresse;
using Patients.Dtos.Classes.Contact;
using Patients.Dtos.Classes.MedicalInfo;
using Patients.Dtos.Classes.Patient;
using Patients.Services.Interfaces;

namespace Patients.Api.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [Route("api")]
    public class PatientsController : ODataController
    {
        private readonly IPatientService _patientService;
        private readonly IContactService _contactService;
        private readonly IAdresseService _adresseService;
        private readonly IMedicalInfoService _medicalInfoService;

        public PatientsController(IPatientService patientService,IContactService contactService,IAdresseService adresseService,IMedicalInfoService medicalInfoService)
        {
            _patientService = patientService;
            _contactService = contactService;
            _adresseService = adresseService;
            _medicalInfoService = medicalInfoService;
        }

        #region Patients

        ///  <summary>
        ///  Get item with parameter passed in the query.
        ///  </summary>
        ///  <response code="500">Internal server error</response>
        [HttpGet("Patients({key:guid})")]
        [HttpGet("Patients/{key:guid}")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] Guid key)
        {
            return Ok(await _patientService.Get(key));
        }

        ///  <summary>
        ///  Get list  with parameter passed in the query.(Odata filtered)
        ///  </summary>
        ///  <response code="500">Internal server error</response>
        [HttpGet("Patients")]
        [HttpGet("Patients/$count")]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All, MaxOrderByNodeCount = 10)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _patientService.Get());
        }

        ///  <summary>
        ///  Add a list of entities to the database from the body of the api call.(Passed by as an array)
        ///  </summary>
        /// <param name="li"></param>
        ///  <response code="500">Internal server error</response> 
        [HttpPost("Patients")]
        public async Task<IActionResult> Post([FromBody] PatientCreateDto li)
        {
            return Ok(await _patientService.Create(li));
        }

        [HttpPatch("Patients({key:guid})")]
        [HttpPatch("Patients/{key:guid}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] Delta<PatientUpdateDto> entity)
        {
            return Ok(await _patientService.Update(key, entity));
        }

        ///  <summary>
        ///  Delete client from the database.
        ///  </summary>
        /// <param name="key"></param>
        ///  <response code="500">Internal server error</response>
        [HttpDelete("Patients({key:guid})")]
        [HttpDelete("Patients/{key:guid}")]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            return Ok(await _patientService.Delete(key));
        }

        #endregion



        #region Adresses

        
        [EnableQuery]
        [HttpGet("Patients({patientId:guid})/Adresses({id:guid})")]
        public async Task<IActionResult> GetAdresse(Guid patientId, Guid id)
        {
            return Ok(await _adresseService.GetAdresse(patientId,id));
        }

        [EnableQuery]
        [HttpGet("Patients({patientId:guid})/Adresses")]
        public async Task<IActionResult> GetAdresse(Guid patientId)
        {
            return Ok(await _adresseService.GetRelative(patientId));
        }

        [HttpPost("Patients({patientId:guid})/Adresses")]
        public async Task<IActionResult> CreateAdresse([FromODataUri] Guid patientId, [FromBody] AdresseCreateDto entity)
        {
            return Ok(await _adresseService.Create(patientId, entity));
        }
        
        [HttpPatch("Patients({patientId:guid})/Adresses({id:guid})")]
        public async Task<IActionResult> PatchAdresse([FromODataUri] Guid patientId, [FromODataUri] Guid id, [FromBody] Delta<AdresseDto> entity)
        {
            return Ok(await _adresseService.Update(patientId, id, entity));
        }
        
        [HttpDelete("Patients({patientId:guid})/Adresses({id:guid})")]
        public async Task<IActionResult> DeleteAdresse([FromODataUri] Guid patientId, [FromODataUri] Guid id)
        {
            return Ok(await _adresseService.Delete(patientId, id));
        }

        #endregion


        #region Contact


        [EnableQuery]
        [HttpGet("Patients({patientId:guid})/Contact({id:guid})")]
        public async Task<IActionResult> GetContact(Guid patientId, Guid id)
        {
            return Ok(await _contactService.GetContact(patientId, id));
        }

        [EnableQuery]
        [HttpGet("Patients({patientId:guid})/Contact")]
        public async Task<IActionResult> GetContact(Guid patientId)
        {
            return Ok(await _contactService.GetRelative(patientId));
        }

        [HttpPost("Patients({patientId:guid})/Contact")]
        public async Task<IActionResult> CreateContact([FromODataUri] Guid patientId, [FromBody] ContactCreateDto entity)
        {
            return Ok(await _contactService.Create(patientId, entity));
        }

        [HttpPatch("Patients({patientId:guid})/Contact({id:guid})")]
        public async Task<IActionResult> PatchContact([FromODataUri] Guid patientId, [FromODataUri] Guid id, [FromBody] Delta<ContactDto> entity)
        {
            return Ok(await _contactService.Update(patientId, id, entity));
        }

        [HttpDelete("Patients({patientId:guid})/Contact({id:guid})")]
        public async Task<IActionResult> DeleteContact([FromODataUri] Guid patientId, [FromODataUri] Guid id)
        {
            return Ok(await _contactService.Delete(patientId, id));
        }

        #endregion


        #region Medical Information


        [EnableQuery]
        [HttpGet("Patients({patientId:guid})/MedicalInfo({id:guid})")]
        public async Task<IActionResult> GetMedical(Guid patientId, Guid id)
        {
            return Ok(await _medicalInfoService.GetMedicalInfo(patientId, id));
        }

        [EnableQuery]
        [HttpGet("Patients({patientId:guid})/MedicalInfo")]
        public async Task<IActionResult> GetMedical(Guid patientId)
        {
            return Ok(await _medicalInfoService.GetRelative(patientId));
        }

        [HttpPost("Patients({patientId:guid})/MedicalInfo")]
        public async Task<IActionResult> CreateMedical([FromODataUri] Guid patientId, [FromBody] MedicalInfoCreateDto entity)
        {
            return Ok(await _medicalInfoService.Create(patientId, entity));
        }

        [HttpPatch("Patients({patientId:guid})/MedicalInfo({id:guid})")]
        public async Task<IActionResult> PatchMedical([FromODataUri] Guid patientId, [FromODataUri] Guid id, [FromBody] Delta<MedicalInfoDto> entity)
        {
            return Ok(await _medicalInfoService.Update(patientId, id, entity));
        }

        [HttpDelete("Patients({patientId:guid})/MedicalInfo({id:guid})")]
        public async Task<IActionResult> DeleteMedical([FromODataUri] Guid patientId, [FromODataUri] Guid id)
        {
            return Ok(await _medicalInfoService.Delete(patientId, id));
        }

        #endregion
    }
}
