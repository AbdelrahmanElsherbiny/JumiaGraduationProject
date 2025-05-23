﻿using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
