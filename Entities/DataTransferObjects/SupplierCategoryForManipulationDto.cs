using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public abstract class SupplierCategoryForManipulationDto
    {
        /// <summary>
        ///     Full name of the category that suppliers can be assigned to
        /// </summary>
        [Required(ErrorMessage = "SupplierCategoryName name is a required field.")]
        [MaxLength(50, ErrorMessage = "Maximum length for the SupplierCategoryName is 50 characters.")]
        public string SupplierCategoryName { get; set; }
    }
}