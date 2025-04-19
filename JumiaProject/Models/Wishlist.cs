using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class Wishlist
{
    public int WishlistId { get; set; }
    public int? ProductVariantId { get; set; }
    public int ProductId { get; set; }
    public string UserId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public virtual ApplicationUser User { get; set; }
    public virtual ProductVariant ProductVariant { get; set; } = null!;
}
