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
        readonly UserManager<ApplicationUser> userManager;
        public HomeController(IHome home, ICart cart, UserManager<ApplicationUser> userManager) : base(cart, userManager)
        {
            this.home = home;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = home.GetData();
            return View(homeVM);
        }

        [HttpGet]
        public HomeVM Search(string searchKey)
        {
            return home.Search(searchKey);
            //return View("_HeaderPartial", products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
