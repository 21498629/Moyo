using System.ComponentModel.DataAnnotations;

namespace Moyo.View_Models
{
    public class OrderItemVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Order is required.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;
    }
}