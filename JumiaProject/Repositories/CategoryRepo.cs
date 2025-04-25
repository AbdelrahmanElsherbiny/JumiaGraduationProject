using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class CategoryRepo : ICategory
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
            var category = Context.Categories.Find(id);
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
            return Context.Categories
                .Any(c => c.CategoryName.ToLower() == categoryName.ToLower());
        }

        public async Task<List<Size>> GetSizesByCategory(int categoryId)
        {

            var sizes = await Context.CategorySizes
                .Where(cs => cs.CategoryId == categoryId)
                .Select(cs => cs.Size)
                .ToListAsync();

            return sizes;
        }

        public async Task<List<Brand>> GetBrandsByCategory(int categoryId)
        {
            var brands = await Context.Products
             .Where(p => p.CategoryId == categoryId)
             .Select(p => p.Brand)
             .Distinct()
             .ToListAsync();
            return brands;
        }

        public async Task<List<decimal>> GetProductsByPriceRange(int categoryId)
        {
            var products = await Context.Products
           .Where(p => p.CategoryId == categoryId && p.Price > 0)
           .Select(p => p.Price)
           .Distinct()
           .OrderBy(p => p)
           .ToListAsync();

            var priceRanges = products
                .GroupBy(p => (int)(p / 50))
                .Select(g => new { Min = g.Min(), Max = g.Max() })
                .ToList();

            return priceRanges.Select(r => r.Max).ToList();
        }

        public async Task<List<decimal>> GetProductsByDiscount(int categoryId)
        {
            var discounts = await Context.Products
          .Where(p => p.CategoryId == categoryId && p.Discount.HasValue)
          .Select(p => p.Discount.Value)
          .Distinct()
          .OrderBy(d => d)
          .ToListAsync();

            return discounts.Where(d => d > 0).ToList();
        }

        public async Task<List<Product>> GetProductsByCategoryWithFilters(
            int categoryId,
            List<int>? sizeId,
             List<int>? brandId,
            decimal? minPrice,
            decimal? maxPrice,
            List<decimal>? selectedDiscounts)
        {
            var query = Context.Products.Where(p => p.CategoryId == categoryId);

            if (sizeId != null && sizeId.Any())
            {
                query = query.Where(p => p.ProductVariants.Any(v => sizeId.Contains(v.SizeId)));
            }

            if (brandId != null && brandId.Any())
            {
                query = query.Where(p => brandId.Contains(p.BrandId));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (selectedDiscounts != null && selectedDiscounts.Any())
            {
                query = query.Where(p => p.Discount.HasValue && selectedDiscounts.Contains(p.Discount.Value));
            }

            return await query.ToListAsync();
        }
    }
}
