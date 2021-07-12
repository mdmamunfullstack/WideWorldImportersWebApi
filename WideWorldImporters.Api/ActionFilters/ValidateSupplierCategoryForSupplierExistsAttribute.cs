using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WideWorldImportersWebApi.ActionFilters
{
    public sealed class ValidateSupplierCategoryForSupplierExistsAttribute : IAsyncActionFilter
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;

        public ValidateSupplierCategoryForSupplierExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
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

            var supplierCategoryId = (int) context.ActionArguments["supplierCategoryId"];
            var supplierCategory = await _repository.SupplierCategories.GetSupplierCategoryAsync(supplierCategoryId, trackChanges);
            if (supplierCategory == null)
            {
                _logger.LogError($"Supplier Category with id: {supplierCategoryId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }

            var id = (int) context.ActionArguments["id"];
            var supplier = await _repository.Supplier.GetSupplierForASupplierCategoryAsync(supplierCategoryId, id, trackChanges);
            if (supplier == null)
            {
                _logger.LogError($"Supplier with id: {id} with Supplier Category: {supplierCategoryId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("supplier", supplier);
                await next();
            }
        }
    }
}