using JumiaProject.Models;

namespace JumiaProject.ViewModels
{
    public class BestProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
