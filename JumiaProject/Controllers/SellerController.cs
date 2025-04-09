using JumiaProject.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class SellerController : Controller
    {
        ISeller seller;
        public SellerController(ISeller _seller)
        {
            seller = _seller;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
