using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Patients.Application.DTOs.Adresse;
using Patients.Application.DTOs.Contact;
using Patients.Application.DTOs.MedicalInfo;
using Patients.Application.DTOs.Patient;
using Patients.Application.Interfaces;

namespace Patients.Api.Controllers
{
    [Route("api")]
    public class PatientsController : ODataController
    {
        private readonly IPatientService _patientService;
        private readonly IContactService _contactService;
        private readonly IAdresseService _adresseService;
        private readonly IMedicalInfoService _medicalInfoService;

        public PatientsController(IPatientService patientService,
            IContactService contactService,
            IAdresseService adresseService,
            IMedicalInfoService medicalInfoService)
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
        [HttpGet("Patients/{key}")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] Guid key)
        {
            return Ok(await _patientService.Get(key));
        }

        ///  <summary>
        ///  Get list  with parameter passed in the query.(Odata filtered)
        ///  </summary>
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
        /// <param name="entity"></param>
        [HttpPost("Patients")]
        public async Task<IActionResult> Post([FromBody] PatientCreateDto entity)
        {
            return Ok(await _patientService.Create(entity));
        }

        [HttpPatch("Patients({key})")]
        [HttpPatch("Patients/{key}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] Delta<PatientUpdateDto> entity)
        {
            return Ok(await _patientService.Update(key, entity));
        }

        ///  <summary>
        ///  Delete client from the database.
        ///  </summary>
        /// <param name="key"></param>
        [HttpDelete("Patients({key})")]
        [HttpDelete("Patients/{key}")]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            return Ok(await _patientService.Delete(key));
        }

        #endregion

        
        #region Adresses

        
        [EnableQuery]
        [HttpGet("Patients({patientId})/Adresses({adressId})")]
        public async Task<IActionResult> GetAdresse(Guid patientId, Guid adressId)
        {
            return Ok(await _adresseService.GetAdresse(patientId, adressId));
        }

        [EnableQuery]
        [HttpGet("Patients({patientId})/Adresses")]
        public async Task<IActionResult> GetAdresse(Guid patientId)
        {
            return Ok(await _adresseService.GetRelative(patientId));
        }

        [HttpPost("Patients({patientId})/Adresses")]
        public async Task<IActionResult> CreateAdresse([FromODataUri] Guid patientId, [FromBody] AdresseCreateDto entity)
        {
            return Ok(await _adresseService.Create(patientId, entity));
        }
        
        [HttpPatch("Patients({patientId})/Adresses({adressId})")]
        public async Task<IActionResult> PatchAdresse([FromODataUri] Guid patientId, [FromODataUri] Guid adressId, [FromBody] Delta<AdresseDto> entity)
        {
            return Ok(await _adresseService.Update(patientId, adressId, entity));
        }
        
        [HttpDelete("Patients({patientId})/Adresses({adressId})")]
        public async Task<IActionResult> DeleteAdresse([FromODataUri] Guid patientId, [FromODataUri] Guid adressId)
        {
            return Ok(await _adresseService.Delete(patientId, adressId));
        }

        #endregion


        #region Contact


        [EnableQuery]
        [HttpGet("Patients({patientId})/Contact({contactId})")]
        public async Task<IActionResult> GetContact(Guid patientId, Guid contactId)
        {
            return Ok(await _contactService.GetContact(patientId, contactId));
        }

        [EnableQuery]
        [HttpGet("Patients({patientId})/Contact")]
        public async Task<IActionResult> GetContact(Guid patientId)
        {
            return Ok(await _contactService.GetRelative(patientId));
        }

        [HttpPost("Patients({patientId})/Contact")]
        public async Task<IActionResult> CreateContact([FromODataUri] Guid patientId, [FromBody] ContactCreateDto entity)
        {
            return Ok(await _contactService.Create(patientId, entity));
        }

        [HttpPatch("Patients({patientId})/Contact({contactId})")]
        public async Task<IActionResult> PatchContact([FromODataUri] Guid patientId, [FromODataUri] Guid contactId, [FromBody] Delta<ContactDto> entity)
        {
            return Ok(await _contactService.Update(patientId, contactId, entity));
        }

        [HttpDelete("Patients({patientId})/Contact({contactId})")]
        public async Task<IActionResult> DeleteContact([FromODataUri] Guid patientId, [FromODataUri] Guid contactId)
        {
            return Ok(await _contactService.Delete(patientId, contactId));
        }

        #endregion


        #region Medical Information


        [EnableQuery]
        [HttpGet("Patients({patientId})/MedicalInfo({medicalInfoId})")]
        public async Task<IActionResult> GetMedical(Guid patientId, Guid medicalInfoId)
        {
            return Ok(await _medicalInfoService.GetMedicalInfo(patientId, medicalInfoId));
        }

        [EnableQuery]
        [HttpGet("Patients({patientId})/MedicalInfo")]
        public async Task<IActionResult> GetMedical(Guid patientId)
        {
            return Ok(await _medicalInfoService.GetRelative(patientId));
        }

        [HttpPost("Patients({patientId})/MedicalInfo")]
        public async Task<IActionResult> CreateMedical([FromODataUri] Guid patientId, [FromBody] MedicalInfoCreateDto entity)
        {
            return Ok(await _medicalInfoService.Create(patientId, entity));
        }

        [HttpPatch("Patients({patientId})/MedicalInfo({medicalInfoId})")]
        public async Task<IActionResult> PatchMedical([FromODataUri] Guid patientId, [FromODataUri] Guid medicalInfoId, [FromBody] Delta<MedicalInfoDto> entity)
        {
            return Ok(await _medicalInfoService.Update(patientId, medicalInfoId, entity));
        }

        [HttpDelete("Patients({patientId})/MedicalInfo({medicalInfoId})")]
        public async Task<IActionResult> DeleteMedical([FromODataUri] Guid patientId, [FromODataUri] Guid medicalInfoId)
        {
            return Ok(await _medicalInfoService.Delete(patientId, medicalInfoId));
        }

        #endregion
    }
}
