namespace JumiaProject.Models
{
    public class ProductDetails
    {
            public string Name { get; set; } = null!;

            public string? Description { get; set; }

            public decimal Price { get; set; }

            public int Stock { get; set; }

            public decimal? Discount { get; set; }

            public string? SKU { get; set; }
      
            public string Code { get; set; } = null!;
        

    }
}
