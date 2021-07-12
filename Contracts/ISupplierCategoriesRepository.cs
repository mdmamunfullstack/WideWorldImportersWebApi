using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface ISupplierCategoriesRepository
    {
        Task<IEnumerable<Purchasing_SupplierCategory>> GetAllSupplierCategoriesAsync(bool trackChanges);

        Task<Purchasing_SupplierCategory> GetSupplierCategoryAsync(int supplierCategoryId, bool trackChanges);

        void CreateSupplierCategory(Purchasing_SupplierCategory supplierCategory);

        Task<IEnumerable<Purchasing_SupplierCategory>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges);

        void DeleteSupplierCategory(Purchasing_SupplierCategory supplierCategory);
    }
}