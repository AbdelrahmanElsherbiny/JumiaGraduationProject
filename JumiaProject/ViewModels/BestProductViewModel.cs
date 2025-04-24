using JumiaProject.Models;

namespace JumiaProject.ViewModels
{
    public class BestProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public List<CartItem> CartItems { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int BrandId { get; set; }
    }
}
