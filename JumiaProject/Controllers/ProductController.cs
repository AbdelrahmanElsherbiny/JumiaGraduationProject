using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class ProductController : BaseController
    {
       private readonly IProduct Product;
       private readonly  ICart _cart;
       private readonly  UserManager<ApplicationUser> _userManager;
        public ProductController(IProduct _product, ICart cart, UserManager<ApplicationUser> userManager):base(cart, userManager) 
        {
            Product = _product;
            _cart = cart;
            _userManager = userManager;
        }
        public async Task<IActionResult> ProductDetails(int id)
        {
            string userId = _userManager.GetUserId(User);
            var productDetails = Product.GetProductById(id);
            

            if (productDetails != null)
            {
                var CategoryProducts = Product.GetProductsByCategory(productDetails.CategoryId);
                var BrandProducts = Product.GetProductsByBrand(productDetails.BrandId);
                var cart = await _cart.GetCartByUserId(userId);
                ProductDetailsViewModel data = new ProductDetailsViewModel()
                {
                    Product = productDetails,
                    CartItems = await _cart.GetAllCartItems(userId),
                    CategoryProducts=CategoryProducts,
                    BrandProducts=BrandProducts
                };
                return View(data);
            }

            return NotFound();
        }


        public async Task<IActionResult> GetBestSeller()
        {
            string userId = _userManager?.GetUserId(User);
            var bestSellerProducts = Product.GetBestSeller();  
            var cartItems = await _cart?.GetAllCartItems(userId);
            BestProductViewModel data = new BestProductViewModel()
            {
                Products = bestSellerProducts,
                CartItems = cartItems,
            };
            ViewBag.PageName = "Best Sellers";
            return View(data);
        }
        public async Task<IActionResult> GetMostDiscount()
        {
            string userId = _userManager?.GetUserId(User);
            List<Product> mostDiscountProducts = Product.GetMostDiscount();
            var cartItems = await _cart?.GetAllCartItems(userId);
            BestProductViewModel data = new BestProductViewModel()
            {
                Products = mostDiscountProducts,
                CartItems = cartItems,
            };
            ViewBag.PageName = "Exclusive Offers | Up to 70% off";
            return View("GetBestSeller",data);
        }

        public async Task<IActionResult> GetBrandProducts(int id)
        {
            string userId = _userManager?.GetUserId(User);
            List<Product> mostDiscountProducts = Product.GetProductsByBrand(id);
            var cartItems = await _cart?.GetAllCartItems(userId);
            BestProductViewModel data = new BestProductViewModel()
            {
                Products = mostDiscountProducts,
                CartItems = cartItems,
            };
            ViewBag.PageName = "Brand";
            return View("GetBestSeller", data);
        }

    }
}
