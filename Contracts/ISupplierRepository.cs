using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Purchasing_Supplier>> GetSuppliersAsync(bool trackChanges);

        Task<Purchasing_Supplier> GetSupplierAsync(int supplierId, bool trackChanges);

        Task<IEnumerable<Purchasing_Supplier>> GetSuppliersForASupplierCategoryAsync(int supplierCategoryId, bool trackChanges);

        Task<Purchasing_Supplier> GetSupplierForASupplierCategoryAsync(int supplierCategoryId, int supplierId, bool trackChanges);

        void CreateSupplier(Purchasing_Supplier supplier);

        void CreateSupplierForSupplierCategory(int supplierCategoryId, Purchasing_Supplier supplier);

        void DeleteSupplier(Purchasing_Supplier supplier);
    }
}