using System;
using System.Collections.Generic;

namespace pataatZOpdracht.Models;

public partial class Product
{
    public int Id { get; set; }

    public int? CategoryId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public decimal? Price { get; set; }

    public int? Discount { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Category { get; set; }

    public decimal DiscountCalculate()
    {
        decimal priceAfterDiscount = 0;
        if (Discount != null)
        {
            decimal discountPercentage = (decimal)Discount / 100;
            priceAfterDiscount = Price.Value - (Price.Value * discountPercentage);
            priceAfterDiscount = Math.Round(priceAfterDiscount, 2);

        }
        return priceAfterDiscount;
    }

}
