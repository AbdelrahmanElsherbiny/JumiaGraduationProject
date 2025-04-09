using JumiaProject.Interfaces;
using JumiaProject.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class OrderController : Controller
    {
        IOrder Order;
        public OrderController(IOrder order)
        {
            Order = order;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
