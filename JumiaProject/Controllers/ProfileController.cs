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
    public class ProfileController : Controller
    {
        private readonly JumiaContext context;
        //JumiaContext context = new JumiaContext();
        private readonly IProfile profile;
        private readonly UserManager<ApplicationUser> userManager;
        public ProfileController(IProfile profile, UserManager<ApplicationUser> userManager, JumiaContext context)
        {
            this.profile = profile;
            this.userManager = userManager;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MyAccount()
        {
            //var userId = userManager.GetUserId(User);
            var userId = "3";
            ProfileVM profileVM = profile.GetUserData(userId);
            return View(profileVM);
        }
        public IActionResult Address()
        {
            var userId = "3";
            var message = TempData["RedirectMessage"];
            //var userId = userManager.GetUserId(User);
            ProfileVM profileVM = profile.GetUserData(userId);
            return View(profileVM);
        }
        [HttpGet]
        public IActionResult EditAddress()
        {
            var userId = "3";
            //var userId = userManager.GetUserId(User);
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
                //var userId = userManager.GetUserId(User);
                var user = context.Users.FirstOrDefault(u => u.Id == "3");
                var address = context.Addresses.FirstOrDefault(a => a.UserId == "3");
                if (user == null || address == null)
                {
                    ModelState.AddModelError("", "User or address not found");
                    return View("EditAddress", model);
                }
                else { 
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

    }
}


