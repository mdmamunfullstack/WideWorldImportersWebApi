using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WideWorldImportersWebApi.ActionFilters;

namespace WideWorldImporters.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/{v:apiversion}/suppliers")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public sealed class SuppliersController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;

        public SuppliersController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        ///     Return options
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public IActionResult GetSuppliersOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, PATCH, DELETE");
            return Ok();
        }

        /// <summary>
        ///     Gets a list of all suppliers
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetSuppliers")]
        [ProducesResponseType(typeof(IEnumerable<SupplierDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSuppliers()
        {
            IEnumerable<Purchasing_Supplier> suppliers = await _repository.Supplier.GetSuppliersAsync(false);
            var suppliersDto = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);

            return Ok(suppliersDto);
        }

        /// <summary>
        ///     Get a supplier
        /// </summary>
        /// <param name="id">SupplierId</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "SupplierById")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetSuppliers(int id)
        {
            var supplier = await _repository.Supplier.GetSupplierAsync(id, false);
            if (supplier == null)
            {
                _logger.LogInfo($"Supplier with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var supplierDto = _mapper.Map<SupplierDto>(supplier);

            return Ok(supplierDto);
        }

        /// <summary>
        ///     Creates a supplier
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns>A newly created supplier</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost(Name = "CreateSupplier")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateSupplier([FromBody] SupplierForCreationDto supplier)
        {
            if (supplier == null)
            {
                _logger.LogError("SupplierForCreationDto object sent from client is null.");
                return BadRequest("SupplierForCreationDto object is null");
            }

            var supplierEntity = _mapper.Map<Purchasing_Supplier>(supplier);

            _repository.Supplier.CreateSupplier(supplierEntity);
            await _repository.SaveAsync();

            var supplierToReturn = _mapper.Map<SupplierDto>(supplierEntity);

            return CreatedAtRoute("SupplierById", new {id = supplierToReturn.SupplierId}, supplierToReturn);
        }

        /// <summary>
        ///     Create a supplier for a supplier category
        /// </summary>
        /// <param name="id"></param>
        /// <param name="supplier"></param>
        /// <returns></returns>
        [HttpPost("{id}", Name = "CreateSupplierForSupplierCategory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateSupplierForSupplierCategory(int id, [FromBody] SupplierForCreationDto supplier)
        {
            var supplierCategory = await _repository.SupplierCategories.GetSupplierCategoryAsync(id, false);
            if (supplierCategory == null)
            {
                _logger.LogInfo($"SupplierCategory with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var supplierEntity = _mapper.Map<Purchasing_Supplier>(supplier);

            _repository.Supplier.CreateSupplierForSupplierCategory(id, supplierEntity);
            await _repository.SaveAsync();

            var supplierToReturn = _mapper.Map<SupplierDto>(supplierEntity);
            int supplierCategoryId = id;

            return CreatedAtRoute("GetSupplierForSupplierCategory", new {supplierCategoryId, id = supplierEntity.SupplierId}, supplierToReturn);
        }

        /// <summary>
        ///     Update supplier
        /// </summary>
        /// <param name="id">Supplier</param>
        /// <param name="supplierCategoryId">Supplier Category</param>
        /// <param name="supplier">Payload</param>
        /// <returns></returns>
        [HttpPut("{id}/suppliercategories/{supplierCategoryId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateSupplierCategoryForSupplierExistsAttribute))]
        public async Task<IActionResult> UpdateSupplierForSupplierCategory(int id, int supplierCategoryId, [FromBody] SupplierForUpdateDto supplier)
        {
            // stored in context by ValidateSupplierCategoryForSupplierExistsAttribute action filter to prevent a second retrieval from database
            var supplierEntity = HttpContext.Items["supplier"] as Purchasing_Supplier;

            _mapper.Map(supplier, supplierEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        ///     Partially update a supplier
        /// </summary>
        /// <param name="supplierCategoryId"></param>
        /// <param name="id"></param>
        /// <param name="patchDoc"></param>
        /// <returns></returns>
        [HttpPatch("{id}/suppliercategories/{supplierCategoryId}")]
        [ServiceFilter(typeof(ValidateSupplierCategoryForSupplierExistsAttribute))]
        public async Task<IActionResult> PartiallyUpdateSupplierForSupplierCategory(int id, int supplierCategoryId, [FromBody] JsonPatchDocument<SupplierForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            // stored in context by ValidateSupplierCategoryForSupplierExistsAttribute action filter to prevent a second retrieval from database
            var supplierEntity = HttpContext.Items["supplier"] as Purchasing_Supplier;

            var supplierToPatch = _mapper.Map<SupplierForUpdateDto>(supplierEntity);

            patchDoc.ApplyTo(supplierToPatch, ModelState);

            TryValidateModel(supplierToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(supplierToPatch, supplierEntity);

            await _repository.SaveAsync();

            return NoContent();
        }
    }
}