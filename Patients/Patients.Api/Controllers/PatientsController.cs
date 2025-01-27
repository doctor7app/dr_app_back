using Microsoft.AspNetCore.Mvc;
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
    [Route("api/Patients")]
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
        
        [HttpGet("{key}")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] Guid key)
        {
            var result = await _patientService.Get(key);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }
        
        [HttpGet]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All, MaxOrderByNodeCount = 10)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _patientService.Get());
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PatientCreateDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }

            var result = await _patientService.Create(entity);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [HttpPatch("{key}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] PatientPatchDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }
            var result = await _patientService.Patch(key, entity);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }
        
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var result = await _patientService.Delete(key);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        #endregion
        
        #region Adresses
        
        [EnableQuery]
        [HttpGet("{key}/Adresses/{idAdress}")]
        public async Task<IActionResult> GetAdresse(Guid key, Guid idAdress)
        {
            var result = await _adresseService.GetAdresse(key, idAdress);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [EnableQuery]
        [HttpGet("{key}/Adresses")]
        public async Task<IActionResult> GetAdresse(Guid key)
        {
            return Ok(await _adresseService.GetRelative(key));
        }

        [HttpPost("{key}/Adresses")]
        public async Task<IActionResult> CreateAdresse([FromODataUri] Guid key, [FromBody] AdresseCreateDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }
            var result = await _adresseService.Create(key, entity);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }
        
        [HttpPatch("{key}/Adresses/{idAdress}")]
        public async Task<IActionResult> PatchAdresse([FromODataUri] Guid key, 
            [FromODataUri] Guid idAdress, 
            [FromBody] AdressePatchDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }

            var result = await _adresseService.Patch(key, idAdress, entity);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }
        
        [HttpDelete("{key}/Adresses/{idAdress}")]
        public async Task<IActionResult> DeleteAdresse([FromODataUri] Guid key, [FromODataUri] Guid idAdress)
        {
            var result = await _adresseService.Delete(key, idAdress);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        #endregion
        
        #region Contact
        
        [EnableQuery]
        [HttpGet("{key}/Contacts/{idContact}")]
        public async Task<IActionResult> GetContact(Guid key, Guid idContact)
        {
            var result = await _contactService.GetContact(key, idContact);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [EnableQuery]
        [HttpGet("{key}/Contacts")]
        public async Task<IActionResult> GetContact(Guid key)
        {
            var result = await _contactService.GetRelative(key);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [HttpPost("{key}/Contacts")]
        public async Task<IActionResult> CreateContact([FromODataUri] Guid key, [FromBody] ContactCreateDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }

            var result = await _contactService.Create(key, entity);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [HttpPatch("{key}/Contacts/{idContact}")]
        public async Task<IActionResult> PatchContact([FromODataUri] Guid key, 
            [FromODataUri] Guid idContact, 
            [FromBody] ContactPatchDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }

            var result = await _contactService.Patch(key, idContact, entity);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [HttpDelete("{key}/Contacts/{idContact}")]
        public async Task<IActionResult> DeleteContact([FromODataUri] Guid key, [FromODataUri] Guid idContact)
        {
            var result = await _contactService.Delete(key, idContact);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        #endregion
        
        #region Medical Information


        [EnableQuery]
        [HttpGet("{key}/MedicalInfos/{idMedicalInfo}")]
        public async Task<IActionResult> GetMedical(Guid key, Guid idMedicalInfo)
        {
            var result = await _medicalInfoService.GetMedicalInfo(key, idMedicalInfo);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [EnableQuery]
        [HttpGet("{key}/MedicalInfos")]
        public async Task<IActionResult> GetMedical(Guid key)
        {
            var result = await _medicalInfoService.GetRelative(key);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [HttpPost("{key}/MedicalInfos")]
        public async Task<IActionResult> CreateMedical([FromODataUri] Guid key, [FromBody] MedicalInfoCreateDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }

            var result = await _medicalInfoService.Create(key, entity);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [HttpPatch("{key}/MedicalInfos/{idMedicalInfo}")]
        public async Task<IActionResult> PatchMedical([FromODataUri] Guid key, 
            [FromODataUri] Guid idMedicalInfo, 
            [FromBody] MedicalInfoPatchDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }

            var result = await _medicalInfoService.Patch(key, idMedicalInfo, entity);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        [HttpDelete("{key}/MedicalInfos/{idMedicalInfo}")]
        public async Task<IActionResult> DeleteMedical([FromODataUri] Guid key, [FromODataUri] Guid idMedicalInfo)
        {
            var result = await _medicalInfoService.Delete(key, idMedicalInfo);
            if (result is Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(result);
        }

        #endregion
    }
}