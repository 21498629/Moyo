using System.ComponentModel.DataAnnotations;

namespace Moyo.View_Models
{
    public class RoleVM
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;
    }
}