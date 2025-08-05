using System.ComponentModel.DataAnnotations;

namespace Moyo.View_Models
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
