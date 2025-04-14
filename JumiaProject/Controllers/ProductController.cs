using JumiaProject.Interfaces;
using JumiaProject.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace JumiaProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProduct _product;

        public ProductController(IProduct product)
        {
            _product = product;
        }
        public IActionResult Index()
        {
            return View();
        }
      
        public IActionResult ProductDetails(int id)
        {
            try
            {
                var productDetails = _product.GetProductById(id);
                if (productDetails != null)
                {

                    ViewData["ProductImage"] = productDetails.ProductImages.Where(x=>x.ProductId == id).ToList();
                    return View(productDetails);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "Internal server error");
            }
        }
        //public IActionResult ShowProductDetails(int id)
        //{
        //    var productDetails = productRepo.GetProductById(id);
        //    if (productDetails != null)
        //    {
        //        return View(productDetails);
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}
    }
}
