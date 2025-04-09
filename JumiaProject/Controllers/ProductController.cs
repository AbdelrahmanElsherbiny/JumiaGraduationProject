using JumiaProject.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class ProductController : Controller
    {
        IProduct product;
        public ProductController(IProduct _product)
        {
            product = _product;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
