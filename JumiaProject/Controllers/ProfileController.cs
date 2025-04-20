using System.IO;
using System.Net;
using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authorization;


namespace JumiaProject.Controllers
{
    //[Authorize]
    public class ProfileController : Controller
    {
        private readonly JumiaContext context;
        //JumiaContext context = new JumiaContext();
        private readonly IProfile profile;
        private readonly UserManager<ApplicationUser> userManager;
        public ProfileController(IProfile profile, UserManager<ApplicationUser> userManager, JumiaContext context)
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IWishlist Wishlist;
        private readonly IHome Home;
        private readonly IOrder Order;
        public ProfileController(UserManager<ApplicationUser> _userManager, IWishlist _wishlist, IHome _home, IOrder _order)
        {
            this.profile = profile;
            this.userManager = userManager;
            this.context = context;
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
        public IActionResult MyAccount()
        [HttpPost]
        public IActionResult AddToWishlist(int productId, int productVariantId)
        {
            //var userId = userManager.GetUserId(User);
            var userId = "3";
            ProfileVM profileVM = profile.GetUserData(userId);
            return View(profileVM);
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, loginRequired = true });
            }

            var wishlist = new Wishlist
        public IActionResult Address()
            {
                ProductId = productId,
                ProductVariantId = productVariantId,
                UserId = userId
            };

            Wishlist.AddToWishlist(wishlist);

            return Json(new { success = true });
            var userId = "3";
            var message = TempData["RedirectMessage"];
            //var userId = userManager.GetUserId(User);
            ProfileVM profileVM = profile.GetUserData(userId);
            return View(profileVM);
        }
        [HttpGet]
        public IActionResult EditAddress()
        [HttpPost]
        public IActionResult RemoveFromWishlist(int wishlistId)
        {
            var userId = "3";
            //var userId = userManager.GetUserId(User);
            var profileVM = profile.GetUserData(userId);
            if (profileVM == null)
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
                profileVM = new ProfileVM
            {
                    User = new UserVM(),
                    Address = new AddressVM()
                };
                return RedirectToAction("Login", "Account");
            }
            var orders = Order.GetOrdersByUserId(userId);
            var canceledOrders = Order.GetCanceledOrdersByUserId(userId);
            ViewBag.userId = userId;
            ViewBag.orders = orders;
            ViewBag.canceledOrders = canceledOrders;
            return View();

            return View(profileVM);
        }
        [HttpPost]
        public IActionResult SaveAddress(ProfileVM model)
        public IActionResult CanceledOrders()
        {
            if (!ModelState.IsValid)
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return View("EditAddress", model);
                return RedirectToAction("Login", "Account");
            }
            var orders = Order.GetOrdersByUserId(userId);
            var canceledOrders = Order.GetCanceledOrdersByUserId(userId);
            ViewBag.userId = userId;
            ViewBag.orders = orders;
            ViewBag.canceledOrders = canceledOrders;
            return View();
        }
            try
        public IActionResult OrderDetails(int id)
        {
            var order = Order.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            var userId = UserManager.GetUserId(User);
            if (userId == null)
                //var userId = userManager.GetUserId(User);
                var user = context.Users.FirstOrDefault(u => u.Id == "3");
                var address = context.Addresses.FirstOrDefault(a => a.UserId == "3");
                if (user == null || address == null)
            {
                    ModelState.AddModelError("", "User or address not found");
                    return View("EditAddress", model);
                return RedirectToAction("Login", "Account");
            }
            ViewBag.userId = userId;
            ViewBag.order = order;
            return View();
                else { 
                    user.UserName = model.User.UserName;
                    user.PhoneNumber = model.User.PhoneNumber;
                    address.Street = model.Address.Street;
                    address.City = model.Address.City;
                    address.Country = model.Address.Country;

                    context.SaveChanges();
                    TempData["SuccessMessage"] = "Address updated successfully!";
        }
                return RedirectToAction("Address", "Profile");

        public IActionResult OrderTracking(int id)
        {
            var order = Order.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                return View("EditAddress", model);
                return RedirectToAction("Login", "Account");
            }
            ViewBag.userId = userId;
            ViewBag.order = order;
            return View();
        }

    }
}


