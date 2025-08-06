using System.ComponentModel.DataAnnotations;

namespace Moyo.View_Models
{
    public class ProductCategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}