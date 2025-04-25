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
    public class ProfileController : BaseController
    {
        private readonly JumiaContext context;
        private readonly IProfile profile;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IWishlist Wishlist;
        private readonly IHome Home;
        private readonly IOrder Order;
        private readonly ICart cart;
        public ProfileController(IProfile profile, JumiaContext context, UserManager<ApplicationUser> _userManager, IWishlist _wishlist, IHome _home, IOrder _order,ICart cart):base(cart,_userManager)
        {
            UserManager = _userManager;
            Wishlist = _wishlist;
            Home = _home;
            Order = _order;
            this.profile = profile;
            this.context = context;
            this.cart = cart;
        }
        public IActionResult MyAccount()
        {
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //var userId = "3";
            ProfileVM profileVM = profile.GetUserData(userId);
            return View(profileVM);
        }
        public IActionResult Address()
        {
            //var userId = "3";
            var message = TempData["RedirectMessage"];
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ProfileVM profileVM = profile.GetUserData(userId);
            return View(profileVM);
        }
        [HttpGet]
        public IActionResult EditAddress()
        {
            //var userId = "3";
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var profileVM = profile.GetUserData(userId);
            if (profileVM == null)
            {
                profileVM = new ProfileVM
                {
                    User = new UserVM(),
                    Address = new AddressVM()
                };
            }

            return View(profileVM);
        }
        [HttpPost]
        public IActionResult SaveAddress(ProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditAddress", model);
            }
            try
            {
                var userId = UserManager.GetUserId(User);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                var address = context.Addresses.FirstOrDefault(a => a.UserId == userId);
                if (user == null || address == null)
                {
                    ModelState.AddModelError("", "User or address not found");
                    return View("EditAddress", model);
                }
                else
                {
                    user.UserName = model.User.UserName;
                    user.PhoneNumber = model.User.PhoneNumber;
                    address.Street = model.Address.Street;
                    address.City = model.Address.City;
                    address.Country = model.Address.Country;

                    context.SaveChanges();
                    TempData["SuccessMessage"] = "Address updated successfully!";
                }
                return RedirectToAction("Address", "Profile");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                return View("EditAddress", model);
            }
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


        [HttpPost]
        public IActionResult ToggleWishlist(int productId, int? productVariantId)
        {
            var userId = UserManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, loginRequired = true });
            }
            var existingWishlistItem = Wishlist.GetWishlistItem(userId, productId, productVariantId);

            if (existingWishlistItem != null)
            {
               Wishlist.RemoveFromWishlist(existingWishlistItem.WishlistId);
                return Json(new { success = true, result = false });
            }
            else
            {
                var wishlist = new Wishlist
                {
                    ProductId = productId,
                    ProductVariantId = productVariantId,
                    UserId = userId
                };

                Wishlist.AddToWishlist(wishlist);
                return Json(new { success = true, result = true });
            }
        }

    }
    }



