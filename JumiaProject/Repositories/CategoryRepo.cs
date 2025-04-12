using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

namespace JumiaProject.Repositories
{
    public class CategoryRepo : ICategory
    {
        private readonly JumiaContext context;
        public CategoryRepo(JumiaContext context)
        {
            this.context = context;
        }
        public List<Category> GetCategories()
        {
            List<Category> categories = context.Categories.ToList();
            return categories;
        }
    }
}
