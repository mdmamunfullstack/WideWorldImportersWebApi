using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WideWorldImportersWebApi.ActionFilters;
using WideWorldImportersWebApi.ModelBinders;

namespace WideWorldImportersWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/{v:apiversion}/suppliercategories")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public sealed class SupplierCategoriesController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;

        public SupplierCategoriesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        ///     Gets a list of all supplier categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSupplierCategories()
        {
            var supplierCategories = await _repository.SupplierCategories.GetAllSupplierCategoriesAsync(false);
            var supplierCategoriesDto = _mapper.Map<IEnumerable<SupplierCategoryDto>>(supplierCategories);

            return Ok(supplierCategoriesDto);
        }

        /// <summary>
        ///     Get a supplier category entity
        /// </summary>
        /// <param name="id">SupplierCategoryId</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "SupplierCategoryById")]
        public async Task<IActionResult> GetSupplierCategory(int id)
        {
            var supplierCategory = await _repository.SupplierCategories.GetSupplierCategoryAsync(id, false);
            if (supplierCategory == null)
            {
                _logger.LogInfo($"Supplier Category with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var supplierCategoryDto = _mapper.Map<SupplierCategoryDto>(supplierCategory);
            return Ok(supplierCategoryDto);
        }

        /// <summary>
        ///     Get a list of several supplier categories
        /// </summary>
        /// <param name="ids">SupplierCategoryIds</param>
        /// <returns></returns>
        [HttpGet("collection/({ids})", Name = "SupplierCategoryCollection")]
        public async Task<IActionResult> GetSupplierCategoryCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<int> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }

            var supplierCategoryEntities = await _repository.SupplierCategories.GetByIdsAsync(ids, false);

            if (ids.Count() != supplierCategoryEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }

            var supplierCategoriesToReturn = _mapper.Map<IEnumerable<SupplierCategoryDto>>(supplierCategoryEntities);
            return Ok(supplierCategoriesToReturn);
        }

        /// <summary>
        ///     Get a list of suppliers for a supplier category
        /// </summary>
        /// <param name="id">SupplierCategoryId</param>
        /// <returns></returns>
        [HttpGet("{id}/suppliers")]
        public async Task<IActionResult> GetSuppliersForSupplierCategory(int id)
        {
            var supplier = await _repository.SupplierCategories.GetSupplierCategoryAsync(id, false);
            if (supplier == null)
            {
                _logger.LogError($"SupplierCategory with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var suppliersFromDb = await _repository.Supplier.GetSuppliersForASupplierCategoryAsync(id, false);
            var suppliersDto = _mapper.Map<IEnumerable<SupplierDto>>(suppliersFromDb);

            return Ok(suppliersDto);
        }

        /// <summary>
        ///     Get a supplier for a supplier category
        /// </summary>
        /// <param name="supplierCategoryId">SupplierCategoryId</param>
        /// <param name="id">SupplierId</param>
        /// <returns></returns>
        [HttpGet("{supplierCategoryId}/suppliers/{id}", Name = "GetSupplierForSupplierCategory")]
        public async Task<IActionResult> GetSupplierForSupplierCategory(int supplierCategoryId, int id)
        {
            var supplierCategory = await _repository.SupplierCategories.GetSupplierCategoryAsync(supplierCategoryId, false);
            if (supplierCategory == null)
            {
                _logger.LogError($"SupplierCategory with id: {supplierCategoryId} doesn't exist in the database.");
                return NotFound();
            }

            var supplierDb = await _repository.Supplier.GetSupplierForASupplierCategoryAsync(supplierCategoryId, id, false);
            if (supplierDb == null)
            {
                _logger.LogError($"Supplier with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var supplier = _mapper.Map<SupplierDto>(supplierDb);
            return Ok(supplier);
        }

        /// <summary>
        ///     Create a supplier category
        /// </summary>
        /// <param name="supplierCategory"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateSupplierCategory([FromBody] SupplierCategoryForCreationDto supplierCategory)
        {
            var supplierCategoryEntity = _mapper.Map<Purchasing_SupplierCategory>(supplierCategory);

            _repository.SupplierCategories.CreateSupplierCategory(supplierCategoryEntity);
            await _repository.SaveAsync();

            var supplierCategoryToReturn = _mapper.Map<SupplierCategoryDto>(supplierCategoryEntity);

            return CreatedAtRoute("SupplierCategoryById", new {id = supplierCategoryToReturn.SupplierCategoryId}, supplierCategoryToReturn);
        }

        /// <summary>
        ///     Create many supplier categories
        /// </summary>
        /// <param name="supplierCategoryCollection"></param>
        /// <returns></returns>
        [HttpPost("collection")]
        public async Task<IActionResult> CreateSupplierCategoryCollection([FromBody] IEnumerable<SupplierCategoryForCreationDto> supplierCategoryCollection)
        {
            if (supplierCategoryCollection == null)
            {
                _logger.LogError("Purchasing_SupplierCategory collection sent from client is null.");
                return BadRequest("Purchasing_SupplierCategory collection is null");
            }

            var supplierCategoryEntities = _mapper.Map<IEnumerable<Purchasing_SupplierCategory>>(supplierCategoryCollection);
            foreach (var supplierCategory in supplierCategoryEntities)
            {
                _repository.SupplierCategories.CreateSupplierCategory(supplierCategory);
            }

            await _repository.SaveAsync();

            var supplierCategoryCollectionToReturn = _mapper.Map<IEnumerable<SupplierCategoryDto>>(supplierCategoryEntities);
            var ids = string.Join(",", supplierCategoryCollectionToReturn.Select(s => s.SupplierCategoryId));

            return CreatedAtRoute("SupplierCategoryCollection", new {ids}, supplierCategoryCollectionToReturn);
        }

        /// <summary>
        ///     Delete supplier category
        /// </summary>
        /// <param name="id">supplierCategoryId</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateSupplierCategoryExistsAttribute))]
        public async Task<IActionResult> DeleteSupplierCategory(int id)
        {
            // stored in context by ValidateSupplierCategoryExistsAttribute action filter to prevent a second retrieval from database
            var supplierCategory = HttpContext.Items["supplierCategory"] as Purchasing_SupplierCategory;

            // cascade delete supplier records
            var suppliersFromDb = await _repository.Supplier.GetSuppliersForASupplierCategoryAsync(id, false);
            foreach (var supplier in suppliersFromDb)
            {
                // cascade delete supplier transaction records
                var supplierTransactionsFromDb = await _repository.SupplierTransactions.GetTransactionsForASupplier(supplier.SupplierId, false);
                foreach (var supplierTransaction in supplierTransactionsFromDb)
                {
                    _repository.SupplierTransactions.DeleteSupplierTransaction(supplierTransaction);
                }

                _repository.Supplier.DeleteSupplier(supplier);
            }

            _repository.SupplierCategories.DeleteSupplierCategory(supplierCategory);

            await _repository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        ///     Update supplier category record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="supplierCategory"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateSupplierCategoryExistsAttribute))]
        public async Task<IActionResult> UpdateSupplierCategory(int id, [FromBody] SupplierCategoryForUpdateDto supplierCategory)
        {
            // stored in context by ValidateSupplierCategoryExistsAttribute action filter to prevent a second retrieval from database
            var supplierCategoryEntity = HttpContext.Items["supplierCategory"] as Purchasing_SupplierCategory;

            _mapper.Map(supplierCategory, supplierCategoryEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        ///     Partially update a supplier category record
        /// </summary>
        /// <param name="supplierCategoryId"></param>
        /// <param name="patchDoc"></param>
        /// <returns></returns>
        [HttpPatch("{supplierCategoryId}")]
        [ServiceFilter(typeof(ValidateSupplierCategoryExistsAttribute))]
        public async Task<IActionResult> PartiallyUpdateSupplierForSupplierCategory(int supplierCategoryId, [FromBody] JsonPatchDocument<SupplierCategoryForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            // stored in context by ValidateSupplierCategoryExistsAttribute action filter to prevent a second retrieval from database
            var supplierCategoryEntity = HttpContext.Items["supplierCategory"] as Purchasing_SupplierCategory;

            var supplierCategoryToPatch = _mapper.Map<SupplierCategoryForUpdateDto>(supplierCategoryEntity);

            patchDoc.ApplyTo(supplierCategoryToPatch, ModelState);

            TryValidateModel(supplierCategoryToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(supplierCategoryToPatch, supplierCategoryEntity);

            await _repository.SaveAsync();

            return NoContent();
        }
    }
}