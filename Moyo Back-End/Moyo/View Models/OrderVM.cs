using System.ComponentModel.DataAnnotations;

namespace Moyo.View_Models
{
    public class OrderVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Order number is required.")]
        public string OrderNumber { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "At least one item is required.")]
        public List<OrderItemVM> OrderItems { get; set; } = new List<OrderItemVM>(); 
    }
}