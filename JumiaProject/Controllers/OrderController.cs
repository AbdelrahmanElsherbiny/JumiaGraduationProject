using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class OrderController : BaseController
    {
        IOrder Order;
        ICart cart;
        UserManager<ApplicationUser> userManager;
        public OrderController(IOrder order ,ICart cart ,UserManager<ApplicationUser> userManager) : base(cart, userManager)
        {
            Order = order;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
