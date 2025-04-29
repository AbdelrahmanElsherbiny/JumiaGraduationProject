using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class ShippingMethod
{
    public int ShippingMethodId { get; set; }

    public string ShippingMethodName { get; set; } = null!;

    public string? EstimatedDeliveryTime { get; set; }

    public decimal Cost { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
