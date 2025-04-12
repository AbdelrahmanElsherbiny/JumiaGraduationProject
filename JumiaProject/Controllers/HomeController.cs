using System.Diagnostics;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.Repositories;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHome home;
        public HomeController(IHome home)
        {
            this.home = home;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = home.GetData();
            return View(homeVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
