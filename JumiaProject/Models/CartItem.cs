using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal PriceAtTime { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
    public int? VariantId { get; set; }
    public virtual ProductVariant? Variant { get; set; }

}
