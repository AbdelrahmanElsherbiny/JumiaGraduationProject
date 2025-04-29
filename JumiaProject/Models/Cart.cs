using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
