using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using JumiaProject.ViewModels;
using JumiaProject.Models;
using Microsoft.AspNetCore.Identity;

namespace JumiaProject.Controllers
{

    public class AccountController : Controller
    {
      
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager) {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        private static LoginViewModel loginVM = new LoginViewModel();


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UserEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Email == "adminadmin@gmail.com")
                {
                    loginVM.Email = model.Email;
                    return RedirectToAction("Password");
                }
                ApplicationUser userModel = await userManager.FindByEmailAsync(model.Email);
                if (userModel != null)
                {
                    loginVM.Email = model.Email;
                    return RedirectToAction("Password");
                }
                // string verificationCode = GenerateVerificationCode();   //justfor now roma
                string verificationCode = "1234";

                loginVM.Email = model.Email;
                loginVM.VerificationCode = verificationCode;
                loginVM.CodeSentTime = DateTime.Now;
                ViewBag.UserEmail = model.Email;

                SendVerificationCode(model.Email, verificationCode);
                return RedirectToAction("Verify");
            }
            ModelState.AddModelError("", "Invalid email format!");
                return View(model);
        }

        [HttpGet]
        public ActionResult Verify()
        {
            ViewBag.UserEmail = loginVM.Email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verify(UserCodeViewModel model)
        {
            ViewBag.UserEmail = loginVM.Email;

            if (ModelState.IsValid) {
                if (loginVM.VerificationCode == model.Code && DateTime.Now < loginVM.CodeSentTime.AddMinutes(1))
                {
                    return RedirectToAction("RegisterPassword");
                }
            }
            ViewBag.UserEmail = loginVM.Email;
            ModelState.AddModelError("", "Invalid or expired code. Please request a new code.");
            return View();
        }

        [HttpGet]
        public ActionResult Password()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Password(UserPasswordLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userModel = await userManager.FindByEmailAsync(loginVM.Email);

                if (userModel == null)
                {

                    ModelState.AddModelError("", "User not found.");
                    return View(model);
                }
                if (loginVM.Email == "adminadmin@gmail.com")
                {
                    bool found = await userManager.CheckPasswordAsync(userModel, model.Password);

                    if (found)
                    {
                        await signInManager.SignInAsync(userModel, true);
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect password!");
                    }
                }
                else
                {
                    bool found = await userManager.CheckPasswordAsync(userModel, model.Password);

                    if (found)
                    {
                        await signInManager.SignInAsync(userModel, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect password!");
                    }
                }
            }

            return View(model);
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(1000, 9999).ToString();
        }

        private void SendVerificationCode(string email, string verificationCode)
        {

            string fromEmail = "reemramdaan@gmail.com";
            string fromName = "Roma Store";
            string toEmail = email;

            const string subject = "Your Verification Code";
            string body = $"Your verification code is: {verificationCode}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("reemramdaan@gmail.com", "rajfzxlssdatgvqo")
            };


          //  smtp.Send(fromEmail, toEmail, subject, body);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public ActionResult ResendCode(string email)
        {
           // string verificationCode = GenerateVerificationCode();
            string verificationCode = "1234";  //jsust for now roma
            loginVM.Email = email;
            loginVM.VerificationCode = verificationCode;
            loginVM.CodeSentTime = DateTime.Now;
            SendVerificationCode(email, verificationCode);
            ViewBag.UserEmail = email;
            ViewBag.ShowResendButton = false;
            return View("Verify");
        }

        public ActionResult RegisterPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterPassword(UserPasswordRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Register
                ApplicationUser userModel = new ApplicationUser();
                userModel.Email = loginVM.Email;
                userModel.UserName = loginVM.Email;
                IdentityResult result = await userManager.CreateAsync(userModel, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(userModel, "Customer");
                    await signInManager.SignInAsync(userModel, false);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Incorrect password!");
                return View();
            }
            ModelState.AddModelError("", "Incorrect password!");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        //[HttpPost]
        //public IActionResult FacebookLogin()
        //{
        //    var redirectUrl = Url.Action("FacebookResponse", "Account");
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
        //    return Challenge(properties, "Facebook");
        //}
        //// Callback from Facebook
        //public async Task<IActionResult> FacebookResponse()
        //{
        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //        return RedirectToAction(nameof(Login));

        //    // Sign in if the user already exists
        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    // If user doesn't exist, create a new one
        //    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //    var user = new IdentityUser { UserName = email, Email = email };
        //    var createResult = await _userManager.CreateAsync(user);

        //    if (createResult.Succeeded)
        //    {
        //        await _userManager.AddLoginAsync(user, info);
        //        await _signInManager.SignInAsync(user, isPersistent: false);
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return RedirectToAction(nameof(Login));
        //}
    }

}
