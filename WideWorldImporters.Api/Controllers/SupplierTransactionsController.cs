using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WideWorldImportersWebApi.ActionFilters;
using WideWorldImportersWebApi.Utility;

namespace WideWorldImportersWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/{v:apiversion}/suppliertransactions/{supplierId}/transactions")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public sealed class SupplierTransactionsController : ControllerBase
    {
        private readonly IDataShaper<SupplierTransactionDto> _dataShaper;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly SupplierTransactionLinks _supplierTransactionLinks;

        public SupplierTransactionsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<SupplierTransactionDto> dataShaper, SupplierTransactionLinks supplierTransactionLinks)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _dataShaper = dataShaper;
            _supplierTransactionLinks = supplierTransactionLinks;
        }

        /// <summary>
        ///     Gets the list of transactions for a supplier
        /// </summary>
        /// <param name="supplierId">SupplierId</param>
        /// <param name="supplierParameters">Query string parameters if they exists</param>
        /// <returns></returns>
        [HttpGet]
        [HttpHead]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetTransactionsForSupplier(int supplierId, [FromQuery] SupplierParameters supplierParameters)
        {
            if (!supplierParameters.ValidPaymentMethodRange)
            {
                return BadRequest("Max payment method can't be less than min payment method.");
            }

            var supplier = await _repository.Supplier.GetSupplierAsync(supplierId, false);
            if (supplier == null)
            {
                _logger.LogError($"Supplier with id: {supplierId} doesn't exist in the database.");
                return NotFound();
            }

            var transactionsFromDb = await _repository.SupplierTransactions.GetAllTransactionsForASupplierAsync(supplierId, supplierParameters, false);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(transactionsFromDb.MetaData));

            var transactionsDto = _mapper.Map<IEnumerable<SupplierTransactionDto>>(transactionsFromDb);

            var links = _supplierTransactionLinks.TryGenerateLinks(transactionsDto, supplierParameters.Fields, supplierId, HttpContext);

            return links.HasLinks
                       ? Ok(links.LinkedEntities)
                       : Ok(links.ShapedEntities);
        }

        /// <summary>
        ///     Get a transaction
        /// </summary>
        /// <param name="supplierId">supplierId</param>
        /// <param name="id">supplierTransactionId</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetTransactionForSupplier")]
        public async Task<IActionResult> GetTransactionForSupplier(int supplierId, int id)
        {
            var supplier = await _repository.Supplier.GetSupplierAsync(supplierId, false);
            if (supplier == null)
            {
                _logger.LogError($"Supplier with id: {supplierId} doesn't exist in the database.");
                return NotFound();
            }

            var transactionDb = await _repository.SupplierTransactions.GetSupplierTransactionAsync(supplierId, id, false);
            if (transactionDb == null)
            {
                _logger.LogInfo($"Supplier with id: {supplierId} with transaction with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var supplierTransactionDto = _mapper.Map<SupplierTransactionDto>(transactionDb);
            return Ok(supplierTransactionDto);
        }

        /// <summary>
        ///     Create a supplier transaction
        /// </summary>
        /// <param name="supplierId">supplierId</param>
        /// <param name="supplierTransaction">DTO used whan creating records</param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateSupplierTransaction(int supplierId, [FromBody] SupplierTransactionForCreationDto supplierTransaction)
        {
            var supplierTransactionEntity = _mapper.Map<Purchasing_SupplierTransaction>(supplierTransaction);

            _repository.SupplierTransactions.CreateSupplierTransaction(supplierTransactionEntity);
            await _repository.SaveAsync();

            var supplierTransactionToReturn = _mapper.Map<SupplierTransactionDto>(supplierTransactionEntity);

            return CreatedAtRoute("GetTransactionForSupplier", new {supplierId, id = supplierTransactionToReturn.SupplierTransactionId}, supplierTransactionToReturn);
        }

        /// <summary>
        ///     Create multiple supplier transactions
        /// </summary>
        /// <param name="supplierTransactionCollection">DTO used when creating records</param>
        /// <returns></returns>
        [HttpPost("collection")]
        public async Task<IActionResult> CreateSupplierTransactionCollection([FromBody] IEnumerable<SupplierTransactionForCreationDto> supplierTransactionCollection)
        {
            if (supplierTransactionCollection == null)
            {
                _logger.LogError("Purchasing_SupplierTransaction collection sent from client is null.");
                return BadRequest("Purchasing_SupplierTransaction collection is null");
            }

            var supplierTransactionEntities = _mapper.Map<IEnumerable<Purchasing_SupplierTransaction>>(supplierTransactionCollection);
            foreach (var supplierTransaction in supplierTransactionEntities)
            {
                _repository.SupplierTransactions.CreateSupplierTransaction(supplierTransaction);
            }

            await _repository.SaveAsync();

            var supplierTransactionCollectionToReturn = _mapper.Map<IEnumerable<SupplierTransactionDto>>(supplierTransactionEntities);
            var ids = string.Join(",", supplierTransactionCollectionToReturn.Select(s => s.SupplierTransactionId));

            return CreatedAtRoute("SupplierTransactionCollection", new {ids}, supplierTransactionCollectionToReturn);
        }

        /// <summary>
        ///     Delete a transaction
        /// </summary>
        /// <param name="supplierId">supplierId</param>
        /// <param name="id">supplierTransactionId</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateSupplierTransactionExistsAttribute))]
        public async Task<IActionResult> DeleteSupplierTransaction(int supplierId, int id)
        {
            // stored in context by ValidateSupplierTransactionExistsAttribute action filter to prevent a second retrieval from database
            var supplierTransaction = HttpContext.Items["supplierTransaction"] as Purchasing_SupplierTransaction;

            // no cascading delete constraint in WideWorldImporters database so we must manually 
            // delete any child records assocoiated with the parent supplier category
            var suppliersFromDb = await _repository.Supplier.GetSuppliersForASupplierCategoryAsync(id, false);
            foreach (var supplier in suppliersFromDb)
            {
                _repository.Supplier.DeleteSupplier(supplier);
            }

            _repository.SupplierTransactions.DeleteSupplierTransaction(supplierTransaction);

            await _repository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        ///     Update a supplier transaction
        /// </summary>
        /// <param name="supplierId">supplierId</param>
        /// <param name="id">supplierTransactionId</param>
        /// <param name="supplierTransaction">DTO used when updating a record</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateSupplierTransactionExistsAttribute))]
        public async Task<IActionResult> UpdateSupplierTransaction(int supplierId, int id, [FromBody] SupplierTransactionForUpdateDto supplierTransaction)
        {
            // stored in context by ValidateSupplierTransactionExistsAttribute action filter to prevent a second retrieval from database
            var supplierTransactionEntity = HttpContext.Items["supplierTransaction"] as Purchasing_SupplierTransaction;

            _mapper.Map(supplierTransaction, supplierTransactionEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        ///     Partially update a supplier tranaction
        /// </summary>
        /// <param name="supplierId">supplierId</param>
        /// <param name="id">supplierTransactionId</param>
        /// <param name="patchDoc"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateSupplierTransactionExistsAttribute))]
        public async Task<IActionResult> PartiallyUpdateSupplierTransaction(int supplierId, int id, [FromBody] JsonPatchDocument<SupplierTransactionForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            // stored in context by ValidateSupplierTransactionExistsAttribute action filter to prevent a second retrieval from database
            var supplierTransactionEntity = HttpContext.Items["supplierTransaction"] as Purchasing_SupplierTransaction;

            var supplierTransactionToPatch = _mapper.Map<SupplierTransactionForUpdateDto>(supplierTransactionEntity);

            patchDoc.ApplyTo(supplierTransactionToPatch, ModelState);

            TryValidateModel(supplierTransactionToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(supplierTransactionToPatch, supplierTransactionEntity);

            await _repository.SaveAsync();

            return NoContent();
        }
    }
}