using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moyo.View_Models
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = null!;

        [Column(TypeName = " decimal(5, 2")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Price must be a postive value.")]
        public decimal Price { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select an image")]
        public IFormFile ImageFile { get; set; }

        public int Quantity { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Vendor is required.")]
        public int VendorId { get; set; }

        [Required(ErrorMessage = "Product category is required.")]
        public int ProductCategoryId { get; set; }
    }
}