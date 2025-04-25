using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace JumiaProject.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        private readonly ISeller Seller;
        private readonly IProduct ProductRepo;
        private readonly IOrder OrderRepo;
        private readonly ICategory CategoryRepo;
        private readonly BrandRepo BrandRepo;
        private readonly IWebHostEnvironment WebHostEnvironment;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<SellerController> _logger;

        public SellerController(
            ISeller _seller,
            IProduct _productRepo,
            IOrder _orderRepo,
            ICategory _categoryRepo,
            BrandRepo _brandRepo,
            IWebHostEnvironment _webHostEnvironment,
            SignInManager<ApplicationUser> _signInManager,
            ILogger<SellerController> logger)  // ✅ Injected
        {
            Seller = _seller;
            ProductRepo = _productRepo;
            OrderRepo = _orderRepo;
            CategoryRepo = _categoryRepo;
            BrandRepo = _brandRepo;
            WebHostEnvironment = _webHostEnvironment;
            signInManager = _signInManager;  // ✅ Assigned
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductsList()
        {
            string sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = ProductRepo.GetAllProductsForSeller(sellerId);
            return View(products);
        }

        public IActionResult Orders()
        {
            var orders = OrderRepo.GetOrdersForSeller(User.Identity.Name);
            return View(orders);
        }

        // Replace your current Profile method with this one
        public IActionResult Profile()
        {
            try
            {
                // Get the current user's ID
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    // User is not authenticated
                    return RedirectToAction("SellerLogin", "SellerAccount");
                }

                // Get the seller by ID using the injected Seller repository
                var user = Seller.GetSellerById(userId);

                if (user == null || user.Seller == null)
                {
                    // User exists but is not a seller
                    TempData["ErrorMessage"] = "Seller profile not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Return the Seller model for the view
                return View(user.Seller);
            }
            catch (Exception ex)
            {
                // Log the exception (in a real app)
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(SellerProfileUpdateDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get current seller with all related data
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var currentUser = Seller.GetSellerById(userId);

                    if (currentUser == null || currentUser.Seller == null)
                    {
                        return NotFound();
                    }

                    // Update specific properties from the DTO
                    currentUser.Seller.StoreName = model.StoreName;
                    currentUser.Seller.TaxNumber = model.TaxNumber;
                    currentUser.Seller.BankAccount = model.BankAccount;

                    if (!string.IsNullOrEmpty(model.Email))
                        currentUser.Email = model.Email;

                    if (!string.IsNullOrEmpty(model.PhoneNumber))
                        currentUser.PhoneNumber = model.PhoneNumber;

                    // Save changes
                    bool result = Seller.UpdateSeller(currentUser);
                    Console.WriteLine($"Update result: {result}");

                    if (result)
                    {
                        TempData["SuccessMessage"] = "Profile updated successfully.";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update profile. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in UpdateProfile: {ex.Message}");
                    ModelState.AddModelError("", $"Error updating profile: {ex.Message}");
                }
            }

            // If we get here, something went wrong, redisplay the form with original seller
            var originalSeller = Seller.GetSellerById(User.FindFirstValue(ClaimTypes.NameIdentifier))?.Seller;
            return View("Profile", originalSeller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("SellerLogin", "SellerAccount");
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("SellerLogin", "SellerAccount");
                }

                var model = new ProductViewModel();
                
                // Populate categories dropdown
                model.Categories = CategoryRepo.GetAllCategories()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();

                // Populate brands dropdown
                model.Brands = BrandRepo.GetAllBrands()
                    .Select(b => new SelectListItem
                    {
                        Value = b.BrandId.ToString(),
                        Text = b.BrandName
                    }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading form: {ex.Message}";
                return RedirectToAction("ProductsList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductViewModel model)
        {
            try
            {
                Console.WriteLine("Starting AddProduct POST action...");
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

                // Log all form values
                Console.WriteLine("Form Values:");
                Console.WriteLine($"Name: {model.Name}");
                Console.WriteLine($"SKU: {model.SKU}");
                Console.WriteLine($"Description: {model.Description}");
                Console.WriteLine($"Price: {model.Price}");
                Console.WriteLine($"Stock: {model.Stock}");
                Console.WriteLine($"CategoryId: {model.CategoryId}");
                Console.WriteLine($"BrandId: {model.BrandId}");
                Console.WriteLine($"MainMaterial: {model.MainMaterial}");
                Console.WriteLine($"PrimaryImage: {(model.PrimaryImage != null ? "Present" : "Missing")}");
                Console.WriteLine($"AdditionalImages Count: {(model.AdditionalImages?.Count ?? 0)}");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage)
                                                .ToList();
                    Console.WriteLine("Model validation errors:");
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"- {error}");
                    }

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, errors = errors });
                    }

                    // Repopulate dropdowns for non-AJAX requests
                    model.Categories = CategoryRepo.GetAllCategories()
                        .Select(c => new SelectListItem
                        {
                            Value = c.CategoryId.ToString(),
                            Text = c.CategoryName
                        }).ToList();

                    model.Brands = BrandRepo.GetAllBrands()
                        .Select(b => new SelectListItem
                        {
                            Value = b.BrandId.ToString(),
                            Text = b.BrandName
                        }).ToList();

                    return View(model);
                }

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"User ID: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID is null or empty");
                    return Json(new { success = false, error = "User not authenticated" });
                }

                Console.WriteLine("Creating product object...");
                Console.WriteLine($"Name: {model.Name}");
                Console.WriteLine($"CategoryId: {model.CategoryId}");
                Console.WriteLine($"BrandId: {model.BrandId}");
                Console.WriteLine($"Price: {model.Price}");
                Console.WriteLine($"Stock: {model.Stock}");

                // Create new product
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Discount = model.Discount,
                    Stock = model.Stock,
                    CategoryId = model.CategoryId,
                    BrandId = model.BrandId,
                    MainMaterial = model.MainMaterial,
                    SKU = model.SKU,
                    SellerId = userId,
                    IsApprovedFromAdmin = "Pending",
                    Code = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper(),
                    CreatedAt = DateTime.Now,
                    IsDeleted = false,
                    ProductImages = new List<ProductImage>()
                };

                Console.WriteLine("Product object created successfully");

                // Handle primary image
                if (model.PrimaryImage != null)
                {
                    Console.WriteLine("Processing primary image...");
                    string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, "images", "products");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PrimaryImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    Console.WriteLine($"Saving primary image to: {filePath}");
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PrimaryImage.CopyToAsync(fileStream);
                    }

                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = "/images/products/" + uniqueFileName,
                        IsPrimary = true
                    });
                    Console.WriteLine("Primary image processed successfully");
                }

                // Handle additional images
                if (model.AdditionalImages != null && model.AdditionalImages.Any())
                {
                    Console.WriteLine($"Processing {model.AdditionalImages.Count} additional images...");
                    string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, "images", "products");
                    Directory.CreateDirectory(uploadsFolder);

                    foreach (var image in model.AdditionalImages)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        Console.WriteLine($"Saving additional image to: {filePath}");
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        product.ProductImages.Add(new ProductImage
                        {
                            ImageUrl = "/images/products/" + uniqueFileName,
                            IsPrimary = false
                        });
                    }
                    Console.WriteLine("Additional images processed successfully");
                }

                Console.WriteLine("Calling ProductRepo.AddProduct...");
                try
                {
                    ProductRepo.AddProduct(product);
                    Console.WriteLine("Product added successfully");

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true });
                    }

                    TempData["SuccessMessage"] = "Product added successfully and pending approval.";
                    return RedirectToAction(nameof(ProductsList));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProductRepo.AddProduct: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddProduct action: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, error = ex.Message });
                }

                ModelState.AddModelError("", $"Error adding product: {ex.Message}");

                // Repopulate dropdowns
                model.Categories = CategoryRepo.GetAllCategories()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();

                model.Brands = BrandRepo.GetAllBrands()
                    .Select(b => new SelectListItem
                    {
                        Value = b.BrandId.ToString(),
                        Text = b.BrandName
                    }).ToList();

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ProductDetails(int id)
        {
            try
            {
                string sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var product = ProductRepo.GetProductById(id);

                if (product == null || product.SellerId != sellerId)
                {
                    return NotFound();
                }

                // Populate dropdowns for editing
                ViewBag.Brands = BrandRepo.GetAllBrands()
                    .Select(b => new SelectListItem
                    {
                        Value = b.BrandId.ToString(),
                        Text = b.BrandName
                    }).ToList();

                ViewBag.Categories = CategoryRepo.GetAllCategories()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();

                return View("SellerProductDetails", product);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in ProductDetails: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading product details.";
                return RedirectToAction(nameof(ProductsList));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDetails(Product product, List<IFormFile> newImages, List<int> deletedImageIds)
        {
            try
            {
                string sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var existingProduct = ProductRepo.GetProductById(product.ProductId);

                if (existingProduct == null || existingProduct.SellerId != sellerId)
                {
                    return NotFound();
                }

                // Handle new images if any
                if (newImages != null && newImages.Any())
                {
                    foreach (var image in newImages)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                        string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, "images", "products");
                        Directory.CreateDirectory(uploadsFolder);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        product.ProductImages.Add(new ProductImage
                        {
                            ImageUrl = "/images/products/" + uniqueFileName,
                            IsPrimary = !product.ProductImages.Any() // First image is primary if no other images exist
                        });
                    }
                }

                // Update the product
                bool updateResult = ProductRepo.UpdateProduct(product);

                if (updateResult)
                {
                    TempData["SuccessMessage"] = "Product updated successfully.";
                    return RedirectToAction(nameof(ProductDetails), new { id = product.ProductId });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update the product. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", product.ProductId);
                ModelState.AddModelError("", $"Error updating product: {ex.Message}");
            }

            // If we get here, something went wrong. Repopulate dropdowns and return to the view
            ViewBag.Brands = BrandRepo.GetAllBrands()
                .Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.BrandName
                }).ToList();

            ViewBag.Categories = CategoryRepo.GetAllCategories()
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();

            return View("SellerProductDetails", product);
        }
    }
}