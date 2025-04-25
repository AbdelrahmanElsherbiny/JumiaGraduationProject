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
        public async Task<IActionResult> Index(int id)
        {
            string userId = _userManager?.GetUserId(User);
            if (userId != null)
            {
                var Products = Product.GetProductsByCategory(id);
                var cartItems = await _cart?.GetAllCartItems(userId);
                BestProductViewModel data = new BestProductViewModel()
                {
                    Products = Products,
                    CartItems = cartItems,
                };
                return View(data);
            }
            else
            {
                var Products = Product.GetProductsByCategory(id);
                var cartItems = new List<CartItem>();
                BestProductViewModel data = new BestProductViewModel()
                {
                    Products = Products,
                    CartItems = cartItems,
                };
                return View(data);
            }
        }


    }
}
