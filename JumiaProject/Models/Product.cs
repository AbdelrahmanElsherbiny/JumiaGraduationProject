using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }

    public int BrandId { get; set; }

    public int SoldNumber { get; set; }

    public string SellerId { get; set; }
    public string? SKU { get; set; }
    public string? MainMaterial { get; set; }
    public DateTime? CreatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public string Code { get; set; } = null!;

    public string IsApprovedFromAdmin { get; set; } = null!;

    public decimal? Discount { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Seller Seller { get; set; } = null!;

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
