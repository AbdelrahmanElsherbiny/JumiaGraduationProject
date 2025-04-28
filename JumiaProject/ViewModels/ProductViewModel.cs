using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System;

namespace JumiaProject.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal? Discount { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [Display(Name = "Brand")]
        public int BrandId { get; set; }

        [StringLength(100)]
        [Display(Name = "Main Material")]
        public string MainMaterial { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
        public string SKU { get; set; }

        // System-generated code for the product
        [Required]
        public string Code { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper();

        // Image Upload Properties
        [Display(Name = "Primary Product Image")]
        [Required(ErrorMessage = "Primary product image is required")]
        public IFormFile PrimaryImage { get; set; }

        [Display(Name = "Additional Product Images")]
        public List<IFormFile> AdditionalImages { get; set; }

        // For displaying existing images
        public List<ProductImageViewModel> ExistingImages { get; set; }

        // Dropdown lists for Categories and Brands
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Brands { get; set; }

        public ProductViewModel()
        {
            AdditionalImages = new List<IFormFile>();
            ExistingImages = new List<ProductImageViewModel>();
            Categories = new List<SelectListItem>();
            Brands = new List<SelectListItem>();
        }
    }

    public class ProductImageViewModel
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
    }
}