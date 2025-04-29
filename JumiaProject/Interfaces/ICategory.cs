using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface ICategory
    {
        public List<Category> GetAllCategories();
        public Category GetCategoryById(int id);
        List<Category> SearchCategories(string searchTerm, int pageNum);
        int GetFilteredCategoriesCount(string searchTerm);
        public void AddCategory(Category category);
        public void Update(Category category);
        public void Delete(int id);
        public bool Exists(int id);
        public bool CategoryExists(string categoryName);
        public Task<List<Size>> GetSizesByCategory(int categoryId);
        public Task<List<Brand>> GetBrandsByCategory(int categoryId);
        Task<List<decimal>> GetProductsByPriceRange(int catgoryId);
        Task<List<decimal>> GetProductsByDiscount(int categoryId);
        Task<List<Product>> GetProductsByCategoryWithFilters(
            int categoryId,
            List<int>? sizeId,
            List<int>? brandId,
            decimal? minPrice,
            decimal? maxPrice,
            List<decimal>? minDiscount);
    }
}

