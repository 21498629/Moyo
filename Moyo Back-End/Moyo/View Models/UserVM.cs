using System.ComponentModel.DataAnnotations;

namespace Moyo.View_Models
{
    public class UserVM
    {

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Surname is required.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [EmailAddress]
        public string UserName { get; set; } = string.Empty;

        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;

        [Phone]
        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }

        public string Token { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}