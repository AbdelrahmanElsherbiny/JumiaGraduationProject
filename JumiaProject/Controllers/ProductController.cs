using JumiaProject.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class ProductController : Controller
    {
        IProduct Product;
        public ProductController(IProduct _product)
        {
            Product = _product;
        }
    }
}
