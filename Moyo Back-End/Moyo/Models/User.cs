using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Moyo.Models;

//[Table("Users")]
public class User : IdentityUser<int>
{
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;
}
