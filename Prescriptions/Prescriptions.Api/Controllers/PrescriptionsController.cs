using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Dtos.Prescriptions;
using Prescriptions.Application.Interfaces.Services;

namespace Prescriptions.Api.Controllers
{
    [ApiController]
    [Route("api/prescriptions")]
    public class PrescriptionsController : ODataController
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPrescriptionItemService _prescriptionItemService;
        private readonly IValidator<PrescriptionCreateDto> _prescriptionCreateValidator;
        private readonly IValidator<PrescriptionUpdateDto> _prescriptionUpdateValidator;
        private readonly IValidator<PrescriptionItemCreateDto> _prescriptionItemCreateValidator;
        private readonly IValidator<PrescriptionItemUpdateDto> _prescriptionItemUpdateValidator;

        public PrescriptionsController(IPrescriptionService prescriptionService, 
            IPrescriptionItemService prescriptionItemService,
            IValidator<PrescriptionCreateDto> prescriptionCreateValidator,
            IValidator<PrescriptionUpdateDto> prescriptionUpdateValidator,
            IValidator<PrescriptionItemCreateDto> prescriptionItemCreateValidator,
            IValidator<PrescriptionItemUpdateDto> prescriptionItemUpdateValidator)
        {
            _prescriptionService = prescriptionService;
            _prescriptionItemService = prescriptionItemService;
            _prescriptionCreateValidator = prescriptionCreateValidator;
            _prescriptionUpdateValidator = prescriptionUpdateValidator;
            _prescriptionItemCreateValidator = prescriptionItemCreateValidator;
            _prescriptionItemUpdateValidator = prescriptionItemUpdateValidator;
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
            var checkModel = await _prescriptionCreateValidator.ValidateAsync(dto);

            if (!checkModel.IsValid)
            {
                return BadRequest(checkModel.Errors);
            }
            var result = await _prescriptionService.CreatePrescriptionAsync(dto);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid id,
            [FromBody] Delta<PrescriptionUpdateDto> patch)
        {
            if (patch == null)
            {
                return BadRequest("Invalid object.");
            }

            var updateDto = new PrescriptionUpdateDto();
            patch.Patch(updateDto);

            var checkModel = await _prescriptionUpdateValidator.ValidateAsync(updateDto);

            if (!checkModel.IsValid)
            {
                return BadRequest(checkModel.Errors);
            }

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
            var checkModel = await _prescriptionItemCreateValidator.ValidateAsync(entity);

            if (!checkModel.IsValid)
            {
                return BadRequest(checkModel.Errors);
            }
            var result = await _prescriptionItemService.CreatePrescriptionItem(key, entity);
            return Ok(result);
        }

        [HttpPatch("{key}/items/{idItem}")]
        public async Task<IActionResult> PatchPrescriptionItem([FromODataUri] Guid key,
            [FromODataUri] Guid idItem,
            [FromBody] Delta<PrescriptionItemUpdateDto> entity)
        {
            if (entity == null)
            {
                return BadRequest("Invalid patch object.");
            }
            
            var updateDto = new PrescriptionItemUpdateDto();
            entity.Patch(updateDto);

            var checkModel = await _prescriptionItemUpdateValidator.ValidateAsync(updateDto);

            if (!checkModel.IsValid)
            {
                return BadRequest(checkModel.Errors);
            }
            
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