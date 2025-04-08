using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class ShippingTracking
{
    public int TrackingId { get; set; }

    public int OrderId { get; set; }

    public string TrackingNumber { get; set; } = null!;

    public string Carrier { get; set; } = null!;

    public string? ShippingStatus { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Order Order { get; set; } = null!;
}
