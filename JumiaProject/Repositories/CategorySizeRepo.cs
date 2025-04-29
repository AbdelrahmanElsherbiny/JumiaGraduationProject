using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class CategorySizeRepo: ICategorySize
    {
        private readonly JumiaContext Context;
        public CategorySizeRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public void AddRange(List<CategorySize> categorySizes)
        {
            Context.CategorySizes.AddRange(categorySizes);
            Context.SaveChanges();
        }
    }
}
