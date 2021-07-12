using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public sealed class SupplierCategoriesRepository : RepositoryBase<Purchasing_SupplierCategory>, ISupplierCategoriesRepository
    {
        public SupplierCategoriesRepository(RepositoryContext repositoryContext)
            : base(repositoryContext) { }

        public async Task<IEnumerable<Purchasing_SupplierCategory>> GetAllSupplierCategoriesAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                         .OrderBy(s => s.SupplierCategoryName)
                         .ToListAsync();
        }

        public async Task<Purchasing_SupplierCategory> GetSupplierCategoryAsync(int supplierCategoryId, bool trackChanges)
        {
            return await FindByCondition(s => s.SupplierCategoryId.Equals(supplierCategoryId), trackChanges)
                       .SingleOrDefaultAsync();
        }

        public void CreateSupplierCategory(Purchasing_SupplierCategory supplierCategory) { Create(supplierCategory); }

        public async Task<IEnumerable<Purchasing_SupplierCategory>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges)
        {
            return await FindByCondition(x => ids.Contains(x.SupplierCategoryId), trackChanges)
                       .ToListAsync();
        }

        public void DeleteSupplierCategory(Purchasing_SupplierCategory supplierCategory) { Delete(supplierCategory); }
    }
}