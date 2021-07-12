using System;

namespace Entities.DataTransferObjects
{
    public class SupplierCategoryDto
    {
        public int SupplierCategoryId { get; set; } // SupplierCategoryID (Primary key)

        /// <summary>
        ///     Full name of the category that suppliers can be assigned to
        /// </summary>
        public string SupplierCategoryName { get; set; } // SupplierCategoryName (length: 50)

        public int LastEditedBy { get; set; } // LastEditedBy
        public DateTime ValidFrom { get; set; } // ValidFrom
        public DateTime ValidTo { get; set; } // ValidTo
    }
}