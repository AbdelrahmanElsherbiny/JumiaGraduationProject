using System.Diagnostics;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.Repositories;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHome home;
        readonly ICart cart;
        readonly IProduct _product;
        private readonly  UserManager<ApplicationUser> _userManager;

        readonly UserManager<ApplicationUser> userManager;
        public HomeController(IHome home, ICart cart, UserManager<ApplicationUser> userManager, IProduct product) : base(cart, userManager)
        {
            this.home = home;
            _product = product;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = home.GetData();
            var bestseller = _product.Get6BestSeller();
            ViewBag.Bestseller = bestseller;
            var mostDiscount = _product.GetMostDiscount();
            ViewBag.MostDiscount = mostDiscount;
            string userId = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(userId))
            {
                var recentlyViewed = await _product.Get6RecentlyViewedProductsAsync(userId);
                ViewBag.RecentlyViewed = recentlyViewed;
            }
            else
            {
                ViewBag.RecentlyViewed = new List<Product>();  // تفريغ الـ ViewBag لو مفيش بيانات
            }
            return View(homeVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
