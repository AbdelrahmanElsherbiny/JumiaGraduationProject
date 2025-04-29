using System.Globalization;
using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        IProduct Product;
        IOrder Order;
        ISeller Seller;
        IUser User;
        IAdmin Admin;
        ICategory Category;
        ISize Size;
        ICategorySize CategorySize;

        public AdminController(IProduct _product, IOrder _order, ISeller _seller, IUser _user, IAdmin _admin, ICategory _category, ISize _size, ICategorySize _categorySize)
        {
            Product = _product;
            Order = _order;
            Seller = _seller;
            User = _user;
            Admin = _admin;
            Category = _category;
            Size = _size;
            CategorySize = _categorySize;
        }

        public IActionResult Index(string sectionName= "", int pageNum = 1)
        {
            ViewBag.currentPage = pageNum;
            return View("Index", sectionName);
        }

        public async Task<IActionResult> LoadSection(string sectionName, int pageNum, string searchTerm = "", string statusFilter = "all")
        {
            switch (sectionName)
            {
                case "Dashboard":
                    var data = await LoadDashboardData();
                    return PartialView("AdminDashboardPartial",data);
                case "Products":
                    List<Product> products = Product.SearchProducts(searchTerm, statusFilter, pageNum);
                    ViewBag.CurrentPage = pageNum;
                    ViewBag.TotalPagesCount = (int)Math.Ceiling((double)Product.GetFilteredProductsCount(searchTerm, statusFilter) / 10);
                    ViewBag.SearchTerm = searchTerm;
                    ViewBag.StatusFilter = statusFilter;
                    return PartialView("AdminProductsPartial", products);

                case "Orders":
                    List<Order> orders = Order.SearchOrders(searchTerm, statusFilter, pageNum);
                    ViewBag.CurrentPage = pageNum;
                    ViewBag.TotalPagesCount = (int)Math.Ceiling((double)Order.GetFilteredOrdersCount(searchTerm, statusFilter) / 10);
                    ViewBag.SearchTerm = searchTerm;
                    ViewBag.StatusFilter = statusFilter;
                    return PartialView("AdminOrdersPartial", orders);

                case "Customers":
                    var customers = await User.SearchCustomers(searchTerm, pageNum);
                    ViewBag.CurrentPage = pageNum;
                    ViewBag.TotalPagesCount = (int)Math.Ceiling((double)await User.GetFilteredCustomersCount(searchTerm) / 10);
                    ViewBag.SearchTerm = searchTerm;
                    return PartialView("AdminCustomersPartial", customers);

                case "Sellers":
                    var sellers = await Seller.SearchSellers(searchTerm, pageNum);
                    ViewBag.CurrentPage = pageNum;
                    ViewBag.TotalPagesCount = (int)Math.Ceiling((double)await Seller.GetFilteredSellersCount(searchTerm) / 10);
                    ViewBag.SearchTerm = searchTerm;
                    return PartialView("AdminSellersPartial", sellers);

                case "Categories":
                    List<Category> categories = Category.SearchCategories(searchTerm, pageNum);
                    ViewBag.CurrentPage = pageNum;
                    ViewBag.TotalPagesCount = (int)Math.Ceiling((double)Category.GetFilteredCategoriesCount(searchTerm) / 10);
                    ViewBag.SearchTerm = searchTerm;
                    return PartialView("AdminCategoriesPartial", categories);

                case "Profile":
                    ApplicationUser masterAdmin = Admin.GetMasterAdmin().Result;
                    return PartialView("AdminProfilePartial", masterAdmin);

                default:
                    return PartialView("WelcomeAdminPartial");
            }
        }
        private async Task<DashboardViewModel> LoadDashboardData()
        {
            // Get most viewed products data
            var mostViewedProducts = await GetMostViewedProductsAsync();

            return new DashboardViewModel
            {
                TopProducts = await Product.GetTopSellingProductsAsync(),
                PendingApprovals = await Product.GetPendingApprovalProductsAsync(),
                RecentOrders = await Order.GetRecentOrdersAsync(),
                TotalRevenue = await Order.GetTotalRevenueAsync(),
                TotalProducts = await Product.GetApprovedProductsCountAsync(),
                TotalCustomers = User.GetCustomersCountAsync(),
                TotalSellers = await Seller.GetSellersCountAsync(),
                MonthlySales = await Order.GetMonthlySalesDataAsync(DateTime.Now.Year),
                MultiYearMonthlySales = await Order.GetMultiYearMonthlySalesDataAsync(5),
                CategoryDistribution = await Product.GetCategoryDistributionAsync(),
                MostViewedProducts = mostViewedProducts
            };
        }

        private async Task<List<MostViewedProductViewModel>> GetMostViewedProductsAsync()
        {
            // Query recently viewed products and group by product to find most viewed
            var context = HttpContext.RequestServices.GetService<JumiaContext>();
            var mostViewed = await context.RecentlyViewedProducts
                .GroupBy(r => r.ProductId)
                .Select(g => new MostViewedProductViewModel
                {
                    Product = g.First().Product,
                    ViewCount = g.Count()
                })
                .OrderByDescending(x => x.ViewCount)
                .Take(10)
                .ToListAsync();

            return mostViewed;
        }
        public IActionResult DeleteProduct(int id, int currentPage = 1)
        {
            Product.DeleteProduct(id);
            return RedirectToAction("Index", "Admin", new { sectionName = "Products", pageNum = currentPage });
        }
        public IActionResult ProductDetails(int id, int currentPage)
        {
            var productDetails = Product.GetProductById(id);
            if (productDetails == null)
            {
                return NotFound();
            }
            ViewBag.currentPage = currentPage;
            ViewBag.Id = id;
            ViewBag.Title = productDetails.Name;
            return View("AdminProductDetails", productDetails);
        }
        public IActionResult VerifyProduct(int id, int currentPage)
        {
            Product.VerifyProduct(id);
            return RedirectToAction("Index", "Admin", new { sectionName = "Products", pageNum = currentPage });
        }
        public IActionResult UnVerifyProduct(int id, int currentPage)
        {
            Product.UnVerifyProduct(id);
            return RedirectToAction("Index", "Admin", new { sectionName = "Products", pageNum = currentPage });
        }
        public IActionResult OrderDetails(int id , int pageNum)
        {
            var order1 = Order.GetOrderById(id);
            if (order1 == null)
            {
                return NotFound();
            }
            ViewBag.currentPage = pageNum;
            return View("AdminOrderDetails", order1);
        }
        public async Task<IActionResult> CustomerDetails(string id, int pageNum)
        {
            var user1 = await User.GetUserById(id);
            if (user1 == null)
            {
                return NotFound();
            }
            ViewBag.currentPage = pageNum;
            return View("AdminUserDetails", user1);
        }
        public IActionResult DeleteCustomer(string id, int pageNum)
        {
            User.DeleteUser(id);
            return RedirectToAction("Index", "Admin", new { sectionName = "Customers", pageNum = pageNum });
        }
        public IActionResult BlockSeller(string id, int pageNum)
        {
            Seller.BlockSeller(id);
            return RedirectToAction("Index","Admin", new { sectionName = "Sellers", pageNum = pageNum });
        }
        public IActionResult VerifySeller(string id, int pageNum)
        {
            Seller.Verify(id);
            return RedirectToAction("Index", "Admin", new { sectionName = "Sellers", pageNum = pageNum });
        }
        public IActionResult UnVerifySeller(string id, int pageNum)
        {
            Seller.UnVerify(id);
            return RedirectToAction("Index", "Admin", new { sectionName = "Sellers", pageNum = pageNum });
        }

        [HttpGet]
        public ActionResult WarmUp()
        {
            // Force initialization of commonly used services
            var warmup = Product.GetAllProducts();
            return Content("Warmup complete");
        }
        [HttpGet]
        public IActionResult AddCategory()
        {
            var sizes = Size.GetAllSizes();
            var parentCategories = (Category.GetAllCategories());

            var model = new CategoryViewModel
            {
                AvailableSizes = sizes.Select(s => new SelectListItem
                {
                    Value = s.SizeId.ToString(),
                    Text = s.SizeLabel
                }).ToList(),
                ParentCategories = parentCategories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList()
            };

            return View(model);
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(CategoryViewModel model)
        {
            // Validate duplicate category (case-insensitive)
            if (Category.CategoryExists(model.CategoryName))
            {
                ModelState.AddModelError("CategoryName", "Category name already exists");
            }

            // Validate image upload
            if (Request.Form.Files["imageUpload"] == null || Request.Form.Files["imageUpload"].Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Please upload a category image");
            }

            // Validate duplicate sizes (case-insensitive)
            if (model.NewSizes != null)
            {
                foreach (var size in model.NewSizes.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    if (Size.SizeExists(size))
                    {
                        ModelState.AddModelError("", $"Size '{size}' already exists");
                        break;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                model.AvailableSizes = Size.GetAllSizes().Select(s => new SelectListItem
                {
                    Value = s.SizeId.ToString(),
                    Text = s.SizeLabel
                }).ToList();

                model.ParentCategories = Category.GetAllCategories()
                    .Where(c => c.ParentCategoryId == null)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();

                return View(model);
            }

            // Handle image upload
            var imageFile = Request.Form.Files["imageUpload"];
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var uploadsFolder = Path.Combine("wwwroot", "images");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            Directory.CreateDirectory(uploadsFolder);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            // Create category
            var category = new Category
            {
                CategoryName = model.CategoryName,
                ParentCategoryId = model.ParentCategoryId,
                ImageUrl = "/images/" + uniqueFileName,
                Description = model.Description,
                IsHomeCategory = model.IsHomeCategory
            };

            Category.AddCategory(category);

            // Handle sizes if needed
            if (model.HasSizes)
            {
                // Add selected sizes
                if (model.SelectedSizeIds?.Any() == true)
                {
                    var categorySizes = model.SelectedSizeIds.Select(sizeId => new CategorySize
                    {
                        CategoryId = category.CategoryId,
                        SizeId = sizeId
                    }).ToList();

                    CategorySize.AddRange(categorySizes);
                }

                // Add new sizes
                if (model.NewSizes?.Any() == true)
                {
                    foreach (var newSize in model.NewSizes.Where(s => !string.IsNullOrWhiteSpace(s)))
                    {
                        var size = new Size { SizeLabel = newSize.Trim() };
                        Size.AddSize(size);

                        Size.AddCategorySize(new CategorySize
                        {
                            CategoryId = category.CategoryId,
                            SizeId = size.SizeId
                        });
                    }
                }
            }

            return RedirectToAction("Index", "Admin", new { sectionName = "Categories" });
        }
        [HttpPost]
        public IActionResult ToggleHomeCategory(int categoryId, int pageNum)
        {
            var category = Category.GetCategoryById(categoryId);
            if (category != null)
            {
                category.IsHomeCategory = !category.IsHomeCategory;
                Category.Update(category);
            }
            return RedirectToAction("Index","Admin", new {sectionName="Categories", pageNum = pageNum });
        }
        [HttpGet]
        public async Task<IActionResult> GetMultiYearSalesData(int years)
        {
            var data = await Order.GetMultiYearMonthlySalesDataAsync(years);
            return Json(data);
        }
    }
}