using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.Repositories;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Controllers
{
    public class ProductController : BaseController
    {
       private readonly JumiaContext _context;
       private readonly IProduct Product;
       private readonly  ICart _cart;
        private readonly IWishlist _wishlist;
        private readonly IProfile profile;

        private readonly  UserManager<ApplicationUser> _userManager;
        public ProductController(IProduct _product, ICart cart, UserManager<ApplicationUser> userManager,IProfile profile, IWishlist wishlist,JumiaContext context):base(cart, userManager) 
        {
            Product = _product;
            _cart = cart;
            _userManager = userManager;
            _wishlist= wishlist;
            _context = context;

            this.profile = profile;
        }
        public async Task<IActionResult> ProductDetails(int id)
        {
            string userId = _userManager.GetUserId(User);
            var productDetails = Product.GetProductById(id);
            if (productDetails != null)
            {
                var CartItems = new List<CartItem>();
                var WishlistItems = new List<Wishlist>();
                if (userId != null)
                {
                    CartItems = await _cart.GetAllCartItems(userId);
                    WishlistItems=_wishlist.GetWishlist(userId);
                }


                var CategoryProducts = Product.GetProductsByCategory(productDetails.CategoryId);
                var BrandProducts = Product.GetProductsByBrand(productDetails.BrandId);
                var cart = await _cart.GetCartByUserId(userId);


                if (userId != null)
                {
                    var recentlyViewed = new RecentlyViewedProduct
                    {
                        UserId = userId,
                        ProductId = productDetails.ProductId,
                        ViewedAt = DateTime.Now
                    };
                    await Product.AddRecentlyViewedProductAsync(recentlyViewed);  
                }

                ProductDetailsViewModel data = new ProductDetailsViewModel()
                {
                    Product = productDetails,
                    CartItems = CartItems,
                    CategoryProducts = CategoryProducts,
                    BrandProducts = BrandProducts,
                    WishlistItems= WishlistItems,
                };
                return View(data);
            }
            else
            {
                return NotFound();
            }
        }

      
        public async Task<IActionResult> GetBestSeller(int pageIndex = 1, int pageSize = 10)
        {
            string userId = _userManager?.GetUserId(User);
            var cartItems = new List<CartItem>();
            var WishlistItems = new List<Wishlist>();
            if (userId != null)
            {
                WishlistItems = _wishlist.GetWishlist(userId);
                cartItems = await _cart?.GetAllCartItems(userId);
            }
            var bestSellerProducts =  Product.GetBestSeller(pageIndex, pageSize);

            int totalItems = Product.GetBestSellerCount();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            BestProductViewModel data = new BestProductViewModel()
            {
                Products = bestSellerProducts,
                CartItems = cartItems,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                WishlistItems=WishlistItems
            };
            ViewBag.PageName = "Best Sellers";
            return View(data);
        }

        public async Task<IActionResult> GetMostDiscount(int pageIndex = 1, int pageSize = 10)
        {
            string userId = _userManager?.GetUserId(User);
            var cartItems = new List<CartItem>();
            var WishlistItems = new List<Wishlist>();
            if (userId != null)
            {
                cartItems = await _cart?.GetAllCartItems(userId);
                WishlistItems = _wishlist.GetWishlist(userId);
            }
            int totalItems = Product.GetMostDiscountCount();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            List<Product> mostDiscountProducts = Product.GetMostDiscount(pageIndex, pageSize);

            BestProductViewModel data = new BestProductViewModel()
            {
                Products = mostDiscountProducts,
                CartItems = cartItems,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                WishlistItems=WishlistItems
            };
            ViewBag.PageName = "Exclusive Offers | Up to 70% off";
            return View("GetBestSeller", data);
        }

        public async Task<IActionResult> RecentlyViewed(int pageIndex = 1, int pageSize = 10)
        {
            string userId = _userManager?.GetUserId(User);
            var cartItems = new List<CartItem>();

            if (userId != null)
            {
                cartItems = await _cart?.GetAllCartItems(userId);
                var recentlyViewedProducts = await Product.GetRecentlyViewedProductsAsync(userId, pageIndex, pageSize);

                int totalItems = Product.GetRecentlyViewedCount(userId);
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var viewModel = new BestProductViewModel
                {
                    Products = recentlyViewedProducts,  
                    CartItems = cartItems,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                    CurrentPage = pageIndex,
                    PageSize = pageSize
                };

                ViewBag.PageName = "Recently Viewed";
                return View("GetBestSeller", viewModel); 
            }

            return View(new BestProductViewModel());
        }



        public async Task<IActionResult> GetBrandProducts(int id,int pageIndex = 1, int pageSize = 10)
        {
            string userId = _userManager?.GetUserId(User);
            var cartItems = new List<CartItem>();
            var WishlistItems = new List<Wishlist>();
            if (userId != null) {
                 cartItems = await _cart?.GetAllCartItems(userId);
                WishlistItems = _wishlist.GetWishlist(userId);
            }
           
            List<Product> brandProducts = Product.GetProductsByBrand(id,pageIndex,pageSize);
            int totalItems = Product.GetProductsByBrandCount(id);
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);


            BestProductViewModel data = new BestProductViewModel()
            {
                BrandId = id,
                Products = brandProducts,
                CartItems = cartItems,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                WishlistItems=WishlistItems
            };
            ViewBag.PageName = "Brand";
            return View("GetBestSeller", data);
        }



        [HttpGet]
        public IActionResult Search(string query)
        {
            var results = string.IsNullOrWhiteSpace(query)
         ? new List<ProductSearchVM>()
         : Product.SearchProducts(query).Select(p => new ProductSearchVM
         {
             ProductId = p.ProductId,
             Name = p.Name
         })
            .ToList();
            return Json(results);
        }
        [HttpGet]
        public IActionResult IsBrandExist(string brand)
        {
            int result = Product.IsExistBrand(brand);
            return Json(new {result = result});
        }

    }
}
