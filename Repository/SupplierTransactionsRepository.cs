using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;

namespace Repository
{
    public sealed class SupplierTransactionsRepository : RepositoryBase<Purchasing_SupplierTransaction>, ISupplierTransactionsRepository
    {
        public SupplierTransactionsRepository(RepositoryContext repositoryContext)
            : base(repositoryContext) { }

        public async Task<PagedList<Purchasing_SupplierTransaction>> GetAllSupplierTransactionsAsync(SupplierParameters supplierParameters, bool trackChanges)
        {
            var transactions = await FindAll(trackChanges)
                                     .Sort(supplierParameters.OrderBy)
                                     .ToListAsync();

            return PagedList<Purchasing_SupplierTransaction>
                .ToPagedList(transactions, supplierParameters.PageNumber, supplierParameters.PageSize);
        }

        public async Task<Purchasing_SupplierTransaction> GetSupplierTransactionAsync(int supplierId, int supplierTransactionId, bool trackChanges)
        {
            return await FindByCondition(t => t.SupplierId.Equals(supplierId) && t.SupplierTransactionId.Equals(supplierTransactionId), trackChanges)
                       .SingleOrDefaultAsync();
        }

        /// <summary>
        ///     Return transactions that pass filter and search conditions
        /// </summary>
        /// <param name="supplierId">Supplier</param>
        /// <param name="supplierParameters">Parameters to filter and search results by</param>
        /// <param name="trackChanges">EF track changes flag</param>
        /// <returns></returns>
        public async Task<PagedList<Purchasing_SupplierTransaction>> GetAllTransactionsForASupplierAsync(int supplierId, SupplierParameters supplierParameters, bool trackChanges)
        {
            var transactions = await FindByCondition(t => t.SupplierId.Equals(supplierId), trackChanges)
                                     .FilterByPaymentMethod(supplierParameters.MinPaymentMethod, supplierParameters.MaxPaymentMethod)
                                     .SearchForSupplierInvoiceNumber(supplierParameters.SupplierInvoiceNumber)
                                     .Sort(supplierParameters.OrderBy)
                                     .ToListAsync();

            var count = await FindByCondition(t => t.SupplierId.Equals(supplierId), trackChanges).CountAsync();

            return PagedList<Purchasing_SupplierTransaction>
                .ToPagedList(transactions, supplierParameters.PageNumber, supplierParameters.PageSize, count);
        }

        public void CreateSupplierTransaction(Purchasing_SupplierTransaction supplierTransaction) { Create(supplierTransaction); }

        public async Task<IEnumerable<Purchasing_SupplierTransaction>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges)
        {
            return await FindByCondition(x => ids.Contains(x.SupplierTransactionId), trackChanges)
                       .ToListAsync();
        }

        public async Task<IEnumerable<Purchasing_SupplierTransaction>> GetTransactionsForASupplier(int supplierId, bool trackChanges)
        {
            return await FindByCondition(t => t.SupplierId.Equals(supplierId), trackChanges)
                         .OrderBy(t => t.SupplierTransactionId)
                         .ToListAsync();
        }

        public void DeleteSupplierTransaction(Purchasing_SupplierTransaction supplierTransaction) { Delete(supplierTransaction); }
    }
}