using System.Collections.Generic;
using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;

namespace JumiaProject.Repositories
{
    public class HomeRepo : IHome
    {
        private readonly ICategory category;
        private readonly IProduct product;
        private readonly JumiaContext context;

        public HomeRepo(ICategory category, IProduct product, JumiaContext context)
        {
            this.category = category;
            this.product = product;
            this.context = context;
        }
        public HomeVM GetData()
        {
            List<CategoryVM> categories = GetCategoriesVM();
            return new HomeVM
            {
                SliderItems = GetSlideritems(),
                Categories = categories,
                Products = GetProductsVM(categories)
            };
        }

        public HomeVM Search(string searchkey)
        {
            List<ProductVM> productVMs = new List<ProductVM>();
            if (!string.IsNullOrEmpty(searchkey))
            {
                List<Product> products = product.GetAllProducts().Where(p => p.Name.Contains(searchkey)).ToList();
                if (products != null)
                {
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
                }
            }
            return new HomeVM
            {
                Products = productVMs
            };
        }

        private List<Slider> GetSlideritems()
        {
            return new List<Slider>()
            {
                new Slider
                {
                    ImgURL="~/images/slider/1.jpg",
                    Link=""
                },
                 new Slider
                {
                    ImgURL="/images/slider/3.jpg",
                    Link=""
                }, new Slider
                {
                    ImgURL="~/images/slider/4.jpg",
                    Link=""
                }, new Slider
                {
                    ImgURL="~/images/slider/5.jpg",
                    Link=""
                }, new Slider
                {
                    ImgURL="~/images/slider/6.jpg",
                    Link=""
                }, new Slider
                {
                    ImgURL="~/images/slider/7.jpg",
                    Link=""
                },
            };
        }

        private List<CategoryVM> GetCategoriesVM()
        {
            List<Category> categories = category.GetAllCategories(); // Fixed method call
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


        private List<ProductVM> GetProductsVM(List<CategoryVM> categories)
        {
            List<Product> products = new List<Product>();
            List<ProductVM> productVMs = new List<ProductVM>();
            foreach (CategoryVM categoryVM in categories)
            {
                products = product.GetProductsByCategory(categoryVM.CategoryId).Take<Product>(6).ToList();
                foreach (Product product in products)
                {
                    ProductVM productVM = new ProductVM
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Stock = product.Stock,
                        CategoryId = product.CategoryId,
                        Discount = product.Discount
                    };
                    productVMs.Add(productVM);
                }
                products.Clear();
            }
            return productVMs;
        }

    }
}
