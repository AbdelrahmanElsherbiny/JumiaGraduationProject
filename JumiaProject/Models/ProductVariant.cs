using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class ProductVariant
{
    public int VariantId { get; set; }

    public int ProductId { get; set; }

    public int SizeId { get; set; }

    public string? Color { get; set; }

    public int Stock { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Size Size { get; set; } = null!;
    public virtual Wishlist Wishlist { get; set; } = null!;
}
