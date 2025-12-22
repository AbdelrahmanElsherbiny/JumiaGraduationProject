using JumiaProject.Models;

namespace JumiaProject.ViewModels
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? ImageUrl { get; set; }
        public bool? IsHomeCategory { get; set; }
    }
}
