using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int AddressId { get; set; }

    public int ShippingMethodId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string OrderStatus { get; set; } = null!;

    public decimal Taxes { get; set; }

    public string UserId { get; set; }  
    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ShippingMethod ShippingMethod { get; set; } = null!;

    public virtual ICollection<ShippingTracking> ShippingTrackings { get; set; } = new List<ShippingTracking>();

    public virtual ApplicationUser User { get; set; } = null!;
}
