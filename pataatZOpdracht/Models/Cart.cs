using System;
using System.Collections.Generic;


namespace pataatZOpdracht.Models;

public partial class Cart
{
    public Cart() 
    {
    }
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? ProdId { get; set; }
    public int? Quantity { get; set; }

    public virtual Product? Prod { get; set; }

    public virtual User? User { get; set; }
}
