using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moyo.Models;

[Table("PRODUCT_CATEGORIES")]
public class ProductCategory
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } =string.Empty;

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
