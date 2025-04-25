using JumiaProject.Models;

namespace JumiaProject.ViewModels
{
    public class CategoryProductsFilterVM
    {
        public int CategoryId { get; set; }
        public List<Product> Products { get; set; }
        public List<Size> Sizes { get; set; }
        public List<Brand> Brands { get; set; }
        public List<decimal> PriceRanges { get; set; }
        public List<decimal> Discounts { get; set; }
        public List<int>? SelectedSizeId { get; set; }
        public List<int>? SelectedBrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<decimal>? SelectedDiscount { get; set; }
        public List<CartItem> CartItems { get; set; }
        public decimal MinAvailablePrice { get; set; }
        public decimal MaxAvailablePrice { get; set; }
        public List<Wishlist> WishlistItems { get; set; }
    }
}