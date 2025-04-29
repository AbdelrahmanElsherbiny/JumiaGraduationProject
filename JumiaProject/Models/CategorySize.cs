using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class CategorySize
{
    public int Id { get; set; }

    public int? CategoryId { get; set; }

    public int? SizeId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Size? Size { get; set; }
}
