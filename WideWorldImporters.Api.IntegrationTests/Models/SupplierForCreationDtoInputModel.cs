
using System;

namespace WideWorldImporters.Api.IntegrationTests.Models
{
    public class SupplierForCreationDtoInputModel : SupplierForManipulationDtoInputModel
    {
        /// <summary>
        ///     Supplier&apos;s category
        /// </summary>
        public int SupplierCategoryId { get; set; } // SupplierCategoryID

        /// <summary>
        ///     Primary contact
        /// </summary>
        public int PrimaryContactPersonId { get; set; } // PrimaryContactPersonID

        /// <summary>
        ///     Alternate contact
        /// </summary>
        public int AlternateContactPersonId { get; set; } // AlternateContactPersonID

        /// <summary>
        ///     Standard delivery method for stock items received from this supplier
        /// </summary>
        public int? DeliveryMethodId { get; set; } // DeliveryMethodID

        /// <summary>
        ///     ID of the delivery city for this address
        /// </summary>
        public int DeliveryCityId { get; set; } // DeliveryCityID

        /// <summary>
        ///     ID of the mailing city for this address
        /// </summary>
        public int PostalCityId { get; set; } // PostalCityID

        /// <summary>
        ///     Supplier reference for our organization (might be our account number at the supplier)
        /// </summary>
        public string SupplierReference { get; set; } // SupplierReference (length: 20)

        /// <summary>
        ///     Supplier&apos;s bank account name (ie name on the account)
        /// </summary>
        public string BankAccountName { get; set; } // BankAccountName (length: 50)

        /// <summary>
        ///     Supplier&apos;s bank branch
        /// </summary>
        public string BankAccountBranch { get; set; } // BankAccountBranch (length: 50)

        /// <summary>
        ///     Supplier&apos;s bank account code (usually a numeric reference for the bank branch)
        /// </summary>
        public string BankAccountCode { get; set; } // BankAccountCode (length: 20)

        /// <summary>
        ///     Supplier&apos;s bank account number
        /// </summary>
        public string BankAccountNumber { get; set; } // BankAccountNumber (length: 20)

        /// <summary>
        ///     Supplier&apos;s bank&apos;s international code (such as a SWIFT code)
        /// </summary>
        public string BankInternationalCode { get; set; } // BankInternationalCode (length: 20)

        /// <summary>
        ///     Number of days for payment of an invoice (ie payment terms)
        /// </summary>
        public int PaymentDays { get; set; } // PaymentDays

        /// <summary>
        ///     Internal comments (not exposed outside organization)
        /// </summary>
        public string InternalComments { get; set; } // InternalComments

        /// <summary>
        ///     Phone number
        /// </summary>
        public string PhoneNumber { get; set; } // PhoneNumber (length: 20)

        /// <summary>
        ///     Fax number
        /// </summary>
        public string FaxNumber { get; set; } // FaxNumber (length: 20)

        /// <summary>
        ///     URL for the website for this supplier
        /// </summary>
        public string WebsiteUrl { get; set; } // WebsiteURL (length: 256)

        /// <summary>
        ///     First delivery address line for the supplier
        /// </summary>
        public string DeliveryAddressLine1 { get; set; } // DeliveryAddressLine1 (length: 60)

        /// <summary>
        ///     Second delivery address line for the supplier
        /// </summary>
        public string DeliveryAddressLine2 { get; set; } // DeliveryAddressLine2 (length: 60)

        /// <summary>
        ///     Delivery postal code for the supplier
        /// </summary>
        public string DeliveryPostalCode { get; set; } // DeliveryPostalCode (length: 10)

        /// <summary>
        ///     First postal address line for the supplier
        /// </summary>
        public string PostalAddressLine1 { get; set; } // PostalAddressLine1 (length: 60)

        /// <summary>
        ///     Second postal address line for the supplier
        /// </summary>
        public string PostalAddressLine2 { get; set; } // PostalAddressLine2 (length: 60)

        /// <summary>
        ///     Postal code for the supplier when sending by mail
        /// </summary>
        public string PostalPostalCode { get; set; } // PostalPostalCode (length: 10)

        public int LastEditedBy { get; set; } // LastEditedBy

        public SupplierForCreationDtoInputModel CloneWith(Action<SupplierForCreationDtoInputModel> changes)
        {
            var clone = (SupplierForCreationDtoInputModel)MemberwiseClone();

            changes(clone);

            return clone;
        }
    }
}