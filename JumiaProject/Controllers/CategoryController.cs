using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class CategoryController : BaseController
    {

        private readonly IProduct Product;
        private readonly ICart _cart;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategory category;
        public CategoryController(IProduct _product, ICart _cart, UserManager<ApplicationUser> userMnager, ICategory category) : base(_cart, userMnager)
        {
            Product = _product;
            this._cart = _cart;
            _userManager = userMnager;
            this.category = category;

        }
        //public async Task<IActionResult> Index(int id)
        //{
        //    string userId = _userManager?.GetUserId(User);
        //    if (userId != null)
        //    {
        //        var Products = Product.GetProductsByCategory(id);
        //        var cartItems = await _cart?.GetAllCartItems(userId);
        //        BestProductViewModel data = new BestProductViewModel()
        //        {
        //            Products = Products,
        //            CartItems = cartItems,
        //        };
        //        return View(data);
        //    }
        //    else
        //    {
        //        var Products = Product.GetProductsByCategory(id);
        //        var cartItems =new List<CartItem>();
        //        BestProductViewModel data = new BestProductViewModel()
        //        {
        //            Products = Products,
        //            CartItems = cartItems,
        //        };
        //        return View(data);
        //    }
        //}


        public async Task<IActionResult> Index(int id, List<int>? sizeIds, List<int>? brandIds, decimal? minPrice, decimal? maxPrice, List<decimal>? discountList)
        {
            string userId = _userManager?.GetUserId(User);
            var cartItems = new List<CartItem>();
            if (userId != null)
            {
                cartItems = await _cart?.GetAllCartItems(userId);
            }
            var availableSizes = await category.GetSizesByCategory(id);
            var availableBrands = await category.GetBrandsByCategory(id);
            var discountOptions = await category.GetProductsByDiscount(id);
            var priceRanges = await category.GetProductsByPriceRange(id);
            List<Product> products;

            if ((sizeIds == null || !sizeIds.Any()) && (brandIds == null || !brandIds.Any()) && !minPrice.HasValue && !maxPrice.HasValue && (discountList == null || !discountList.Any()))
            {
                products = Product.GetProductsByCategory(id);
            }
            else
            {
                products = await category.GetProductsByCategoryWithFilters(id, sizeIds, brandIds, minPrice, maxPrice, discountList);
            }
            decimal minAvailablePrice = 0;
            decimal maxAvailablePrice = 0;

            if (priceRanges.Any())
            {
                minAvailablePrice = priceRanges.Min();
                maxAvailablePrice = priceRanges.Max();
            }


            CategoryProductsFilterVM data = new CategoryProductsFilterVM()
            {
                CategoryId = id,
                Products = products,
                Sizes = availableSizes,
                Brands = availableBrands,
                PriceRanges = priceRanges,
                Discounts = discountOptions,
                SelectedSizeId = sizeIds ?? new List<int>(),
                SelectedBrandId = brandIds,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SelectedDiscount = discountList,
                CartItems = cartItems,
                MaxAvailablePrice = maxAvailablePrice,
                MinAvailablePrice = minAvailablePrice,

            };

            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> FilterProducts(
            int id,
            List<int>? sizeIds,
            List<int>? brandIds,
            List<decimal>? discountList,
            decimal? minPrice,
            decimal? maxPrice
            )
        {
            var availableSizes = await category.GetSizesByCategory(id);
            var availableBrands = await category.GetBrandsByCategory(id);
            var discountOptions = await category.GetProductsByDiscount(id);
            var priceRanges = await category.GetProductsByPriceRange(id);


            var filteredProducts = await category.GetProductsByCategoryWithFilters(
                id,
                sizeIds,
                brandIds,
                minPrice,
                maxPrice,
                discountList
            );
            if (filteredProducts.Count == 0)
            {
                Console.WriteLine("No products found");
            }

            decimal minAvailablePrice = 0;
            decimal maxAvailablePrice = 0;
            if (priceRanges.Any())
            {
                minAvailablePrice = priceRanges.Min();
                maxAvailablePrice = priceRanges.Max();
            }

            var viewModel = new CategoryProductsFilterVM
            {
                Products = filteredProducts,
                Sizes = availableSizes,
                Brands = availableBrands,
                Discounts = discountOptions,
                PriceRanges = priceRanges,
                SelectedSizeId = sizeIds ?? new List<int>(),
                SelectedBrandId = brandIds,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SelectedDiscount = discountList,
                CartItems = new List<CartItem>()
            };

            return PartialView("_FilteredProductsPartial", viewModel);
        }
    }
}
