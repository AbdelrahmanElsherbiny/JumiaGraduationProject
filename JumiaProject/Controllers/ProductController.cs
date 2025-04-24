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


        public async Task<IActionResult> GetBestSeller(int pageIndex = 1, int pageSize = 10)
        {
            string userId = _userManager?.GetUserId(User);
            var cartItems = new List<CartItem>();
            if (userId != null)
            {

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
                PageSize = pageSize
            };
            ViewBag.PageName = "Best Sellers";
            return View(data);
        }

        public async Task<IActionResult> GetMostDiscount(int pageIndex = 1, int pageSize = 10)
        {
            string userId = _userManager?.GetUserId(User);
            var cartItems = new List<CartItem>();
            if (userId != null)
            {
                cartItems = await _cart?.GetAllCartItems(userId);
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
                PageSize = pageSize
            };
            ViewBag.PageName = "Exclusive Offers | Up to 70% off";
            return View("GetBestSeller", data);
        }
        public async Task<IActionResult> GetBrandProducts(int id,int pageIndex = 1, int pageSize = 10)
        {
            string userId = _userManager?.GetUserId(User);
            var cartItems = await _cart?.GetAllCartItems(userId);

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
                PageSize = pageSize
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
