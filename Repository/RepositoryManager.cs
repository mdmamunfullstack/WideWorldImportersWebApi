using System.Threading.Tasks;
using Contracts;
using Entities;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private ISupplierCategoriesRepository _supplierCategoriesRepository;
        private ISupplierRepository _supplierRepository;
        private ISupplierTransactionsRepository _supplierTransactionRepository;

        public RepositoryManager(RepositoryContext repositoryContext) { _repositoryContext = repositoryContext; }

        public ISupplierCategoriesRepository SupplierCategories
        {
            get
            {
                if (_supplierCategoriesRepository == null)
                {
                    _supplierCategoriesRepository = new SupplierCategoriesRepository(_repositoryContext);
                }

                return _supplierCategoriesRepository;
            }
        }

        public ISupplierRepository Supplier
        {
            get
            {
                if (_supplierRepository == null)
                {
                    _supplierRepository = new SupplierRepository(_repositoryContext);
                }

                return _supplierRepository;
            }
        }

        public ISupplierTransactionsRepository SupplierTransactions
        {
            get
            {
                if (_supplierTransactionRepository == null)
                {
                    _supplierTransactionRepository = new SupplierTransactionsRepository(_repositoryContext);
                }

                return _supplierTransactionRepository;
            }
        }

        public Task SaveAsync() { return _repositoryContext.SaveChangesAsync(); }
    }
}