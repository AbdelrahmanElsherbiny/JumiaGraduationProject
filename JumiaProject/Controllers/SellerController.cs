using JumiaProject.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class SellerController : Controller
    {
        ISeller Seller;
        public SellerController(ISeller _seller)
        {
            Seller = _seller;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
