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
    }
}
