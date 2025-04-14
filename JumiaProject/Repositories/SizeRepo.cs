using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class SizeRepo:ISize
    {
        private readonly JumiaContext Context;
        public SizeRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public  List<Size> GetAllSizes()
        {
            return  Context.Sizes.ToList();
        }

        public void AddSize(Size size)
        {
            Context.Sizes.Add(size);
            Context.SaveChanges();
        }

        public void AddCategorySize(CategorySize categorySize)
        {
            Context.CategorySizes.Add(categorySize);
            Context.SaveChanges();
        }
        public bool SizeExists(string sizeLabel)
        {
            return  Context.Sizes
                .Any(s => s.SizeLabel.ToLower() == sizeLabel.ToLower());
        }
    }
}
