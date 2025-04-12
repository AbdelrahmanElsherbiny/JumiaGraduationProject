using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;

namespace JumiaProject.Repositories
{
    public class HomeRepo : IHome
    {
        private readonly ICategory category;
        private readonly IProduct product;
        public HomeRepo(ICategory category, IProduct product)
        {
            this.category = category;
            this.product = product;
        }
        public HomeVM GetData()
        {
            return new HomeVM
            {
                SliderItems = GetSlideritems(),
                Categories = GetCategoriesVM(),
                Products = GetProductsVM()
            };
        }

        private List<Slider> GetSlideritems()
        {
            return new List<Slider>()
            {
                new Slider
                {
                    ImgURL="/images/1.jpg",
                    Link=""
                },
                 new Slider
                {
                    ImgURL="/images/2.jpg",
                    Link=""
                },
                  new Slider
                {
                    ImgURL="/images/3.jpg",
                    Link=""
                }
            };
        }

        private List<CategoryVM> GetCategoriesVM()
        {
            List<Category> categories = category.GetCategories();
            List<CategoryVM> categoriesVM = new List<CategoryVM>();

            foreach (Category category in categories)
            {
                CategoryVM categoryVM = new CategoryVM
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    ImageUrl = category.ImageUrl
                };
                categoriesVM.Add(categoryVM);
            }
            return categoriesVM;
        }

        private List<ProductVM> GetProductsVM()
        {
            List<Product> products = product.GetAllProducts().Where(p => p.IsDeleted == false && p.IsApprovedFromAdmin == "Approved").ToList();
            List<ProductVM> productVMs = new List<ProductVM>();
            foreach (Product product in products)
            {
                ProductVM productVM = new ProductVM
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    CategoryId = product.CategoryId
                };
                productVMs.Add(productVM);
            }
            return productVMs;
         }
    }
}
