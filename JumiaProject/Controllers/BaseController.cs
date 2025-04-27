using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JumiaProject.Controllers
{
    public class BaseController : Controller
    {
        private readonly ICart cart;
        private readonly UserManager<ApplicationUser> userManager;

        public BaseController(ICart cart, UserManager<ApplicationUser> userManager)
        {
            this.cart = cart;
            this.userManager = userManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string userId = userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(userId))
            {
                Cart myCart = await cart.GetCartByUserId(userId);
                var cartCount = await cart.GetTotalCartQuantity(myCart.CartId);
                ViewBag.CartCount = cartCount;
            }
            else
            {
                ViewBag.CartCount = 0;
            }

            await next(); // Proceed to the next step in the pipeline
        }
    }

}

