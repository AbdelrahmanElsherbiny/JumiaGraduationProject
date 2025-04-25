using JumiaProject.Models;

namespace JumiaProject.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public List<CartItem> CartItems { get; set; }
        public List<Product> CategoryProducts { get; set; }
        public List<Product> BrandProducts {  get; set; }

    }
}
