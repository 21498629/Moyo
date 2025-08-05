using Moyo.Models;
using System.ComponentModel.DataAnnotations;

namespace Moyo.View_Models
{
    public class VendorVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = null!;

        [Phone]
        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; } = null!;
    }
}
