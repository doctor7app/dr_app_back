using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Prescriptions.Application.Dtos.Events;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Dtos.Prescriptions;
using Prescriptions.Application.Interfaces;

namespace Prescriptions.Api.Controllers
{
    [ApiController]
    [Route("api/prescriptions")]
    public class PrescriptionsController : ODataController
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPrescriptionItemService _prescriptionItemService;

        public PrescriptionsController(IPrescriptionService prescriptionService, IPrescriptionItemService prescriptionItemService)
        {
            _prescriptionService = prescriptionService;
            _prescriptionItemService = prescriptionItemService;
        }

        #region Prescriptions

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
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromODataUri] Guid id)
        {
            var result = await _prescriptionService.DeletePrescriptionAsync(id);
            return result ? NoContent() : NotFound();
        }
        

        #endregion


        #region Items

        [EnableQuery]
        [HttpGet("{key}/items/{idItem}")]
        public async Task<IActionResult> GetPrescriptionItem(Guid key, Guid idItem)
        {
            var result = await _prescriptionItemService.GetItemByIdAsync(key,idItem);
            return Ok(result);
        }

        [EnableQuery]
        [HttpGet("{key}/items")]
        public async Task<IActionResult> GetPrescriptionItem(Guid key)
        {
            var result = await _prescriptionItemService.GetAllItemRelatedToPrescriptionByIdAsync(key);
            return Ok(result);
        }

        [HttpPost("{key}/items")]
        public async Task<IActionResult> CreatePrescriptionItem([FromODataUri] Guid key, [FromBody] PrescriptionItemCreateDto entity)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Merci de vérifier les données saisie !");
            }

            var result = await _prescriptionItemService.CreatePrescriptionItem(key, entity);
            return Ok(result);
        }

        [HttpPatch("{key}/items/{idItem}")]
        public async Task<IActionResult> PatchPrescriptionItem([FromODataUri] Guid key,
            [FromODataUri] Guid idItem,
            [FromBody] Delta<PrescriptionItemUpdateDto> entity)
        {
            var result = await _prescriptionItemService.UpdateItemAsync(key, idItem, entity);
            
            return Ok(result);
        }


        [HttpDelete("{key}/items/{idItem}")]
        public async Task<IActionResult> DeletePrescriptionItem([FromODataUri] Guid key, [FromODataUri] Guid idItem)
        {
            var result = await _prescriptionItemService.DeleteItemAsync(key, idItem);
           
            return Ok(result);
        }

        #endregion
    }
}