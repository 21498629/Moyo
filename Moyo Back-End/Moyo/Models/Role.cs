using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Moyo.Models
{
    //[Table("ROLES")]
    public class Role : IdentityRole<int>
    {
    }
}
