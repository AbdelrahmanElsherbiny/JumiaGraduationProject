using System;
using System.Collections.Generic;

namespace JumiaProject.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int? ParentCategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public bool? IsHomeCategory { get; set; }

    public virtual ICollection<CategorySize> CategorySizes { get; set; } = new List<CategorySize>();

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
