using JumiaProject.Models;

namespace JumiaProject.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int CategoryId { get; set; }
        public Decimal? Discount { get; set; }
    }
}
