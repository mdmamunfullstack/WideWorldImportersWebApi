using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WideWorldImportersWebApi.ActionFilters
{
    public class ValidateSupplierTransactionExistsAttribute : IAsyncActionFilter
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;

        public ValidateSupplierTransactionExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var trackChanges = method.Equals("PUT") || method.Equals("PATCH")
                                   ? true
                                   : false;

            var supplierId = (int) context.ActionArguments["supplierId"];
            var supplier = await _repository.Supplier.GetSupplierAsync(supplierId, false);

            if (supplier == null)
            {
                _logger.LogInfo($"Supplier with id: {supplierId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }

            var id = (int) context.ActionArguments["id"];
            var supplierTransaction = await _repository.SupplierTransactions.GetSupplierTransactionAsync(supplierId, id, trackChanges);

            if (supplierTransaction == null)
            {
                _logger.LogError($"SupplierTransaction with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("supplierTransaction", supplierTransaction);
                await next();
            }
        }
    }
}