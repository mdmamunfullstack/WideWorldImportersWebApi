using System;

namespace Entities.DataTransferObjects
{
    public class SupplierTransactionDto
    {
        /// <summary>
        ///     Numeric ID used to refer to a supplier transaction within the database
        /// </summary>
        public int SupplierTransactionId { get; set; } // SupplierTransactionID (Primary key)

        /// <summary>
        ///     Supplier for this transaction
        /// </summary>
        public int SupplierId { get; set; } // SupplierID

        /// <summary>
        ///     Type of transaction
        /// </summary>
        public int TransactionTypeId { get; set; } // TransactionTypeID

        /// <summary>
        ///     ID of an purchase order (for transactions associated with a purchase order)
        /// </summary>
        public int? PurchaseOrderId { get; set; } // PurchaseOrderID

        /// <summary>
        ///     ID of a payment method (for transactions involving payments)
        /// </summary>
        public int? PaymentMethodId { get; set; } // PaymentMethodID

        /// <summary>
        ///     Invoice number for an invoice received from the supplier
        /// </summary>
        public string SupplierInvoiceNumber { get; set; } // SupplierInvoiceNumber (length: 20)

        /// <summary>
        ///     Date for the transaction
        /// </summary>
        public DateTime TransactionDate { get; set; } // TransactionDate

        /// <summary>
        ///     Transaction amount (excluding tax)
        /// </summary>
        public decimal AmountExcludingTax { get; set; } // AmountExcludingTax

        /// <summary>
        ///     Tax amount calculated
        /// </summary>
        public decimal TaxAmount { get; set; } // TaxAmount

        /// <summary>
        ///     Transaction amount (including tax)
        /// </summary>
        public decimal TransactionAmount { get; set; } // TransactionAmount

        /// <summary>
        ///     Amount still outstanding for this transaction
        /// </summary>
        public decimal OutstandingBalance { get; set; } // OutstandingBalance

        /// <summary>
        ///     Date that this transaction was finalized (if it has been)
        /// </summary>
        public DateTime? FinalizationDate { get; set; } // FinalizationDate

        /// <summary>
        ///     Is this transaction finalized (invoices, credits and payments have been matched)
        /// </summary>
        public bool? IsFinalized { get; private set; } // IsFinalized

        public int LastEditedBy { get; set; } // LastEditedBy
        public DateTime LastEditedWhen { get; set; } // LastEditedWhen
    }
}