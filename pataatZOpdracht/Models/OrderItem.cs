using System;
using System.Collections.Generic;

namespace pataatZOpdracht.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }
}
