using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IWishlist Wishlist;
        private readonly IHome Home;
        private readonly IOrder Order;
        public ProfileController(UserManager<ApplicationUser> _userManager, IWishlist _wishlist, IHome _home, IOrder _order)
        {
            UserManager = _userManager;
            Wishlist = _wishlist;
            Home = _home;
            Order = _order;
        }
        public IActionResult UserWishlist()
        {
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var wishlistItems = Wishlist.GetWishlist(userId);
            ViewBag.userId = userId;
            ViewBag.wishlistItems = wishlistItems;
            return View();
        }
        [HttpPost]
        public IActionResult AddToWishlist(int productId, int productVariantId)
        {
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, loginRequired = true });
            }

            var wishlist = new Wishlist
            {
                ProductId = productId,
                ProductVariantId = productVariantId,
                UserId = userId
            };

            Wishlist.AddToWishlist(wishlist);

            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult RemoveFromWishlist(int wishlistId)
        {
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, loginRequired = true });
            }
            Wishlist.RemoveFromWishlist(wishlistId);
            return Json(new { success = true });
        }
        public IActionResult UserOrders()
        {
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var orders = Order.GetOrdersByUserId(userId);
            var canceledOrders = Order.GetCanceledOrdersByUserId(userId);
            ViewBag.userId = userId;
            ViewBag.orders = orders;
            ViewBag.canceledOrders = canceledOrders;
            return View();
        }
        public IActionResult CanceledOrders()
        {
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var orders = Order.GetOrdersByUserId(userId);
            var canceledOrders = Order.GetCanceledOrdersByUserId(userId);
            ViewBag.userId = userId;
            ViewBag.orders = orders;
            ViewBag.canceledOrders = canceledOrders;
            return View();
        }
        public IActionResult OrderDetails(int id)
        {
            var order = Order.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.userId = userId;
            ViewBag.order = order;
            return View();
        }
        public IActionResult OrderTracking(int id)
        {
            var order = Order.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.userId = userId;
            ViewBag.order = order;
            return View();
        }
    }
}
