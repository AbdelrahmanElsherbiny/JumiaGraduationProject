using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JumiaProject.Models;

public partial class Seller
{
    [ForeignKey("User")]
    public string SellerId { get; set; } = null!;

    public string StoreName { get; set; } = null!;

    public int TaxNumber { get; set; }

    public int? Rating { get; set; }

    public int? BankAccount { get; set; }

    public bool? IsVerified { get; set; }

    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
