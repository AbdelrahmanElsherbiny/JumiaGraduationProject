using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class CategoryRepo:ICategory
    {
        private readonly JumiaContext Context;
        public CategoryRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public List<Category> GetAllCategories()
        {
            return Context.Categories.ToList();
        }
        public Category GetCategoryById(int id)
        {
            return Context.Categories.FirstOrDefault(c => c.CategoryId == id);
        }
        public List<Category> SearchCategories(string searchTerm, int pageNum)
        {
            var query = GetAllCategories()
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c =>
                    c.CategoryName.ToLower().Contains(searchTerm.ToLower()));
            }

            return query
                .OrderBy(c => c.CategoryId)
                .ThenBy(c => c.CategoryName)
                .Skip((pageNum - 1) * 10)
                .Take(10)
                .ToList();
        }

        public int GetFilteredCategoriesCount(string searchTerm)
        {
            var query = GetAllCategories()
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c =>
                    c.CategoryName.ToLower().Contains(searchTerm.ToLower()));
            }

            return query.Count();
        }
        public void AddCategory(Category category)
        {
            Context.Categories.Add(category);
            Context.SaveChanges();
        }
        public void Delete(int id)
        {
            var category =  Context.Categories.Find(id);
            if (category != null)
            {
                Context.Categories.Remove(category);
                Context.SaveChanges();
            }
        }

        public bool Exists(int id)
        {
            return Context.Categories.Any(e => e.CategoryId == id);
        }
        public void Update(Category category)
        {
            Context.Update(category);
            Context.SaveChanges();
        }
        public bool CategoryExists(string categoryName)
        {
            return  Context.Categories
                .Any(c => c.CategoryName.ToLower() == categoryName.ToLower());
        }
    }
}
