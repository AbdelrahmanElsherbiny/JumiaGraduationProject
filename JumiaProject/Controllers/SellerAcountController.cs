using System.Net.Mail;
using System.Net;
using JumiaProject.Models;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace JumiaProject.Controllers
{
    public class SellerAccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public SellerAccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        private static RegisterSellerViewModel registerVM= new RegisterSellerViewModel();

        [HttpGet]
        public IActionResult SelectCountry()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SelectCountry(SellerCountryViewModel model)
        {
            if (ModelState.IsValid)
            {  
                registerVM.Country = model.Country;
                return RedirectToAction("EnterEmail");
            }
            ModelState.AddModelError("", "Please select a country.");
            return View();
        }

        public ActionResult EnterEmail()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnterEmail(SellerEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userModel = await userManager.FindByEmailAsync(model.Email);
                if (userModel != null)
                {
                    registerVM.Email = model.Email;
                    ModelState.AddModelError("", "Email is already Exist!");
                    return View(model);
                }

                string verificationCode = GenerateVerificationCode();
                registerVM.Email = model.Email;
                registerVM.CodeSentTime = DateTime.Now;
                registerVM.VerificationCode = "1234";   /////just until now Roma
                ViewBag.UserEmail = model.Email;
                SendVerificationCode(model.Email, verificationCode);
                return RedirectToAction("VerifyCode");
            }
            ModelState.AddModelError("", "Invalid email format!");
            return View();
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


          //  smtp.Send(fromEmail, toEmail, subject, body);    ////just for now roma
        }

        [HttpGet]
        public IActionResult VerifyCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyCode(SellerCodeViewModel model)
        {
            ViewBag.UserEmail = registerVM.Email;

            if (ModelState.IsValid)
            {
                if (registerVM.VerificationCode == model.Code && DateTime.Now < registerVM.CodeSentTime.AddMinutes(5))
                {
                    return RedirectToAction("EnterPassword");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid or expired code. Please request a new code.");
                    return View();
                }
            }

            ViewBag.UserEmail = registerVM.Email;
            ModelState.AddModelError("", "Incorrect verification code.");
            return View();
        }
        [HttpPost]
        public ActionResult ResendCode(string email)
        {
            string verificationCode = GenerateVerificationCode();
            registerVM.Email = email;
            //registerVM.VerificationCode = verificationCode;
            registerVM.VerificationCode = "1234";  //just for now roma
            registerVM.CodeSentTime = DateTime.Now;
            SendVerificationCode(email, verificationCode);
            ViewBag.UserEmail = email;
            return View("VerifyCode");
        }

        [HttpGet]
        public IActionResult EnterPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnterPassword(SellerPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                registerVM.Password = model.Password;
                registerVM.Phone = model.Phone;
                return RedirectToAction("ShopDetails"); 
            }
            ModelState.AddModelError("", "Please fill all required fields.");
            return View(model);
        }
        
        [HttpGet]
        public IActionResult ShopDetails()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ShopDetails(SellerShopDetailsViewModel model)
        {
                if (ModelState.IsValid&& model.IsPolicyAccepted==true)
                {
                    // Fill view model
                    registerVM.ShopType = model.ShopType;
                    registerVM.ShopName = model.ShopName;
                    registerVM.ShippingAddress = model.ShippingAddress;
                    registerVM.IsPolicyAccepted = model.IsPolicyAccepted;

                    // Create user
                    var userModel = new ApplicationUser
                    {
                        Email = registerVM.Email,
                        UserName = registerVM.Email.Split("@")[0],
                        PhoneNumber = registerVM.Phone,
                        Seller = new Seller
                        {
                            Rating = 4,
                            TaxNumber = 12345,
                            StoreName = registerVM.ShopName,
                        }
                    };

                    IdentityResult result = await userManager.CreateAsync(userModel, registerVM.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(userModel, "Seller");
                        await signInManager.SignInAsync(userModel, isPersistent: false);
                        return RedirectToAction("Index", "Home");    // just for now until seller dashboard
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                }

                }
            ModelState.AddModelError("", "Please fill out all fields and accept the policy, Sure That you accept our Policy.");
            return View(model);
        }




        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("SellerLogin");
        }

        [HttpGet]
        public IActionResult SellerLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SellerLogin(SellerLoginViewModel seller)
        {
            if (ModelState.IsValid) {
                ApplicationUser userModel = await userManager.FindByEmailAsync(seller.Email);
                bool found = await userManager.CheckPasswordAsync(userModel, seller.Password);
                if (found)
                {
                    await signInManager.SignInAsync(userModel, seller.RememberMe);
                    return RedirectToAction("Index", "Home");   ////just for now Roma until seller panel
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email or Password!");
                    ViewBag.ErrorMessage = "Invalid Email or Password!";
                    return View(seller);
                  
                }
            }
            return View(seller);
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(SellerEmailViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found with this email.");
                    return View(model);
                }
                var code = GenerateVerificationCode();
                registerVM.CodeSentTime = DateTime.Now;
                registerVM.VerificationCode = "1234"; //code;   //just for now roma
                registerVM.Email = model.Email;
                ViewBag.UserEmail = model.Email;
                SendVerificationCode(model.Email, "1234");
                return RedirectToAction("VerifyCodeForgetPassword");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult VerifyCodeForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyCodeForgetPassword(SellerCodeViewModel model)
        {
            ViewBag.UserEmail = registerVM.Email;

            if (ModelState.IsValid)
            {
                if (registerVM.VerificationCode == model.Code && DateTime.Now < registerVM.CodeSentTime.AddMinutes(5))
                {
                    return RedirectToAction("SetNewPassword");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid or expired code");
                    return View();
                }
            }

            ViewBag.UserEmail = registerVM.Email;
            ModelState.AddModelError("", "Incorrect verification code.");
            return View();
        }

        [HttpGet]
        public IActionResult SetNewPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetNewPassword(SellerResetNewPasswordViewModel model)
        {
            if (ModelState.IsValid) {
                if (string.IsNullOrEmpty(registerVM.Email))
                {
                    return RedirectToAction("ForgetPassword");
                }
                var user = await userManager.FindByEmailAsync(registerVM.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found with this email.");
                    return View("ForgetPassword");
                }
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("SellerLogin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
    }
}
