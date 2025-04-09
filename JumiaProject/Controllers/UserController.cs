using JumiaProject.Interfaces;
using JumiaProject.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class UserController : Controller
    {
        IUser User;
        public UserController(IUser user)
        {
            User = user;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
