using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moyo.Models;

[Table("ORDERS")]
public class Order
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
