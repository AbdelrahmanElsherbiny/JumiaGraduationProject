using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class ProductRepo : IProduct
    {
        JumiaContext Context;
        public ProductRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public List<Product> GetAllProducts()
        {
            return Context.Products.Where(p => p.IsApprovedFromAdmin != "Not Approved" && p.IsDeleted == false).ToList();
        }
        public List<Product> GetProductsPaginated(int pageNumber)
        {
            int pageSize = 10;
            return Context.Products.Where(p=>p.IsApprovedFromAdmin != "Not Approved" && p.IsDeleted == false).OrderByDescending(p=>p.IsApprovedFromAdmin).ThenBy(p=>p.ProductId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
        public void VerifyProduct(int id)
        {
            var product = Context.Products.FirstOrDefault(p => p.ProductId == id);
            product.IsApprovedFromAdmin = "Approved";
            Context.SaveChanges();
        }
        public void UnVerifyProduct(int id)
        {
            var product = Context.Products.FirstOrDefault(p => p.ProductId == id);
            product.IsApprovedFromAdmin = "Not Approved";
            Context.SaveChanges();
        }
        public Product GetProductByName(string name)
        {
            return Context.Products.FirstOrDefault(p => p.Name == name);
        }
        public Product GetProductById(int id)
        {
            return Context.Products.Where(p => p.IsDeleted == false).FirstOrDefault(p => p.ProductId == id);
        }
        public bool AddProduct(Product product)
        {
                Context.Products.Add(product);
                Context.SaveChanges();
                return true;
        }
        public void DeleteProduct(int id)
        {
            var product = Context.Products.FirstOrDefault(p => p.ProductId == id);
                product.IsDeleted = true;
                Context.SaveChanges();
        }
        public bool UpdateProduct(Product product)
        {
            Context.Products.Update(product);
            Context.SaveChanges();
            return true;
        }
        public List<Product> GetProductsByCategory(int id)
        {
            var category = Context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category.ParentCategoryId==null)
            {
                var subcategories = Context.Categories.Where(c => c.ParentCategoryId == id).Select(c => c.CategoryId).ToList();
                return Context.Products.Where(p => subcategories.Contains(p.CategoryId)).ToList();
            }
            else 
            {
                return Context.Products.Where(p => p.Category.CategoryId == id).ToList();
            }
        }
        public List<Product> GetProductsByBrand(int id)
        {
            return Context.Products.Where(p => p.BrandId == id).ToList();
        }
        public List<Product> SearchProducts(string searchTerm, string statusFilter, int pageNum)
        {
            var query = Context.Products.Where(p=>p.IsDeleted == false && p.IsApprovedFromAdmin != "Not Approved").AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>p.Name.ToLower().Contains(searchTerm.ToLower()) && p.IsDeleted == false);
            }

            if (statusFilter != "all")
            {
                if (statusFilter == "available")
                {
                    query = query.Where(p => p.Stock > 0 && p.IsDeleted == false);
                }
                else if (statusFilter == "out-of-stock")
                {
                    query = query.Where(p => p.Stock <= 0 && p.IsDeleted == false);
                }
                else if (statusFilter == "pending")
                {
                    query = query.Where(p => p.IsApprovedFromAdmin == "Pending");
                }
            }

            return query
                .OrderBy(p => p.ProductId)
                .Skip((pageNum - 1) * 10)
                .Take(10)
                .ToList();
        }

        public int GetFilteredProductsCount(string searchTerm, string statusFilter)
        {
            var query = Context.Products.Where(p => p.IsDeleted == false && p.IsApprovedFromAdmin != "Not Approved").AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>p.Name.ToLower().Contains(searchTerm.ToLower()) && p.IsDeleted == false);
            }

            if (statusFilter != "all")
            {
                if (statusFilter == "available")
                {
                    query = query.Where(p => p.Stock > 0 && p.IsDeleted == false);
                }
                else if (statusFilter == "out-of-stock")
                {
                    query = query.Where(p => p.Stock <= 0 && p.IsDeleted == false);
                }
                else if (statusFilter == "pending")
                {
                    query = query.Where(p => p.IsApprovedFromAdmin == "Pending");
                }
            }

            return query.Count();
        }
        public  List<Product> Get6BestSeller()
        {
            var bestseller = Context.Products.OrderByDescending(p => p.SoldNumber).Take(6).ToList();
            return bestseller;
        }
        public List<Product> GetBestSeller()
        {
            var bestseller = Context.Products.OrderByDescending(p => p.SoldNumber).ToList();
            return bestseller;
        }
        public List<Product> GetMostDiscount()
        {
            var bestseller = Context.Products.Where(p=>p.Discount!=0).OrderByDescending(p => p.Discount).ToList();
            return bestseller;
        }
        public async Task<List<Product>> GetTopSellingProductsAsync(int count = 5)
        {
            return await Context.Products
                .Where(p => p.IsDeleted != true && p.IsApprovedFromAdmin == "Approved")
                .OrderByDescending(p => p.SoldNumber)
                .Take(count)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToListAsync();
        }

        public async Task<List<Product>> GetPendingApprovalProductsAsync(int count = 5)
        {
            return await Context.Products
                .Where(p => p.IsDeleted != true && p.IsApprovedFromAdmin == "Pending")
                .Include(p => p.Seller)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetApprovedProductsCountAsync()
        {
            return await Context.Products
                .Where(p => p.IsDeleted != true && p.IsApprovedFromAdmin == "Approved")
                .CountAsync();
        }

        public async Task<Dictionary<string, int>> GetCategoryDistributionAsync()
        {
            // Get the count of approved products grouped by category
            var categoryDistribution = await Context.Products
                .Where(p => p.IsApprovedFromAdmin == "Approved")
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToListAsync();

            // Create dictionary with category names and product counts
            var distribution = new Dictionary<string, int>();

            foreach (var item in categoryDistribution)
            {
                var category = await Context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryId == item.CategoryId);

                if (category != null)
                {
                    distribution[category.CategoryName] = item.Count;
                }
            }

            // Return only top 8 categories for better visualization
            // Sort by count descending and take top categories
            return distribution
                .OrderByDescending(x => x.Value)
                .Take(8)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
