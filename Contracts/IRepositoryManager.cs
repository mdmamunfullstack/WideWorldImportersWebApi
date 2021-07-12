using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        ISupplierCategoriesRepository SupplierCategories { get; }
        ISupplierRepository Supplier { get; }
        ISupplierTransactionsRepository SupplierTransactions { get; }

        Task SaveAsync();
    }
}