using JumiaProject.Models;

namespace JumiaProject.ViewModels
{
    public class CategoryProductsFilterVM
    {
        public int CategoryId { get; set; }
        public List<Product> Products { get; set; }

    }
}