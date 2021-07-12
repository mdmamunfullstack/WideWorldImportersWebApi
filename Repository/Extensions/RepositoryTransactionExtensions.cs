using System.Linq;
using System.Linq.Dynamic.Core;
using Entities;
using Repository.Extensions.Utility;

namespace Repository.Extensions
{
    public static class RepositoryTransactionExtensions
    {
        /// <summary>
        ///     Filter results by payment method 1 to 4 - seems only type 4 exist in WideWorldImports database
        /// </summary>
        /// <param name="transactions">All possible transactions</param>
        /// <param name="minPaymentMethod">Minimum payment method</param>
        /// <param name="maxPaymentMethod">Maximum payment method</param>
        /// <returns></returns>
        public static IQueryable<Purchasing_SupplierTransaction> FilterByPaymentMethod(this IQueryable<Purchasing_SupplierTransaction> transactions, uint minPaymentMethod, uint maxPaymentMethod) { return transactions.Where(t => t.PaymentMethodId >= minPaymentMethod && t.PaymentMethodId <= maxPaymentMethod); }

        /// <summary>
        ///     Search results to make sure they include records with a specific supplier invoice number
        /// </summary>
        /// <param name="transactions">All possible transactions</param>
        /// <param name="supplierInvoiceNumber">Supplier invoice number to search for</param>
        /// <returns></returns>
        public static IQueryable<Purchasing_SupplierTransaction> SearchForSupplierInvoiceNumber(this IQueryable<Purchasing_SupplierTransaction> transactions, string supplierInvoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(supplierInvoiceNumber))
            {
                return transactions;
            }

            return transactions.Where(t => t.SupplierInvoiceNumber.Equals(supplierInvoiceNumber));
        }

        /// <summary>
        ///     This code builds a dynamic OrderBy based on query string parameters
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="orderByQueryString"></param>
        /// <returns></returns>
        public static IQueryable<Purchasing_SupplierTransaction> Sort(this IQueryable<Purchasing_SupplierTransaction> transactions, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return transactions.OrderBy(t => t.SupplierTransactionId);
            }

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Purchasing_SupplierTransaction>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
            {
                return transactions.OrderBy(t => t.SupplierTransactionId);
            }

            return transactions.OrderBy(orderQuery);
        }
    }
}