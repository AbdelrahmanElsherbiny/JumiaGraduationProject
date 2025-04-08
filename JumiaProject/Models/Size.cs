using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public string? SizeLabel { get; set; }

    public virtual ICollection<CategorySize> CategorySizes { get; set; } = new List<CategorySize>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
