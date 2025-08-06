using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moyo.Models;

[Table("ORDER_ITEMS")]
public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    //[Column("ProductID")]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
