using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Entities.RequestFeatures;

namespace Contracts
{
    public interface ISupplierTransactionsRepository
    {
        Task<PagedList<Purchasing_SupplierTransaction>> GetAllSupplierTransactionsAsync(SupplierParameters supplierParameters, bool trackChanges);

        Task<PagedList<Purchasing_SupplierTransaction>> GetAllTransactionsForASupplierAsync(int supplierId, SupplierParameters supplierParameters, bool trackChanges);

        Task<Purchasing_SupplierTransaction> GetSupplierTransactionAsync(int supplierId, int supplierTransactionId, bool trackChanges);

        Task<IEnumerable<Purchasing_SupplierTransaction>> GetTransactionsForASupplier(int supplierId, bool trackChanges);

        void CreateSupplierTransaction(Purchasing_SupplierTransaction supplierTransaction);

        Task<IEnumerable<Purchasing_SupplierTransaction>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges);

        void DeleteSupplierTransaction(Purchasing_SupplierTransaction supplierTransaction);
    }
}