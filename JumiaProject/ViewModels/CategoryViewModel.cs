using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace JumiaProject.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [Display(Name = "Parent Category")]
        public int? ParentCategoryId { get; set; }

        [Url]
        [StringLength(250)]
        public string? ImageUrl { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Show on Homepage")]
        public bool IsHomeCategory { get; set; }

        [Display(Name = "This category has sizes")]
        public bool HasSizes { get; set; }

        [Display(Name = "Available Sizes")]
        public List<int> SelectedSizeIds { get; set; } = new List<int>();

        [Display(Name = "Add New Sizes")]
        public List<string> NewSizes { get; set; } = new List<string>();

        public List<SelectListItem> ParentCategories { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailableSizes { get; set; } = new List<SelectListItem>();

    }
}
