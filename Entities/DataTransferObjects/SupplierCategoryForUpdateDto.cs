using System.Collections.Generic;

namespace Entities.DataTransferObjects
{
    public class SupplierCategoryForUpdateDto : SupplierCategoryForManipulationDto
    {
        public int LastEditedBy { get; set; } // LastEditedBy
        public IEnumerable<SupplierForCreationDto> Purchasing_Suppliers { get; set; }
    }
}