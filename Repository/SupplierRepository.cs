using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public sealed class SupplierRepository : RepositoryBase<Purchasing_Supplier>, ISupplierRepository
    {
        public SupplierRepository(RepositoryContext repositoryContext)
            : base(repositoryContext) { }

        public async Task<IEnumerable<Purchasing_Supplier>> GetSuppliersAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                         .OrderBy(s => s.SupplierName)
                         .ToListAsync();
        }

        public async Task<Purchasing_Supplier> GetSupplierAsync(int supplierId, bool trackChanges)
        {
            return await FindByCondition(e => e.SupplierId.Equals(supplierId), trackChanges)
                       .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Purchasing_Supplier>> GetSuppliersForASupplierCategoryAsync(int supplierCategoryId, bool trackChanges)
        {
            return await FindByCondition(e => e.SupplierCategoryId.Equals(supplierCategoryId), trackChanges)
                         .OrderBy(e => e.SupplierName)
                         .ToListAsync();
        }

        public async Task<Purchasing_Supplier> GetSupplierForASupplierCategoryAsync(int supplierCategoryId, int supplierId, bool trackChanges)
        {
            return await FindByCondition(e => e.SupplierCategoryId.Equals(supplierCategoryId) && e.SupplierId.Equals(supplierId), trackChanges)
                       .SingleOrDefaultAsync();
        }

        public void CreateSupplier(Purchasing_Supplier supplier) { Create(supplier); }

        public void CreateSupplierForSupplierCategory(int supplierCategoryId, Purchasing_Supplier supplier)
        {
            supplier.SupplierCategoryId = supplierCategoryId;
            Create(supplier);
        }

        public void DeleteSupplier(Purchasing_Supplier supplier) { Delete(supplier); }
    }
}