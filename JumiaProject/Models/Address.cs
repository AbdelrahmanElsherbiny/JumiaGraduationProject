using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public string Country { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string? ZipCode { get; set; }
    public string UserId { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ApplicationUser User { get; set; } = null!;
}
