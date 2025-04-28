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
            return Context.Products.Where(p => p.Category.CategoryId == id).ToList();
        }
        public List<Product> GetProductsByBrand(int id, int pageIndex = 1, int pageSize = 10)
        {
            return Context.Products.Where(p => p.BrandId == id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
        public List<Product> GetBestSeller(int pageIndex = 1, int pageSize = 10)
        {
            var bestseller = Context.Products.OrderByDescending(p => p.SoldNumber).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return bestseller;
        }
        public int GetBestSellerCount()
        {
            var bestsellerCount = Context.Products.OrderByDescending(p => p.SoldNumber).Count();
            return bestsellerCount;
        }
        public int GetMostDiscountCount()
        {
            var mostDiscounCount = Context.Products.Where(p => p.Discount != 0).OrderByDescending(p => p.Discount).Count();
            return mostDiscounCount;
        }
        public int GetProductsByBrandCount(int id)
        {
            var productByBrandCount = Context.Products.Where(b => b.BrandId == id).Count();
            return productByBrandCount;
        }

        public List<Product> GetMostDiscount(int pageIndex = 1, int pageSize = 10)
        {
            var bestseller = Context.Products.Where(p=>p.Discount!=0).OrderByDescending(p => p.Discount).Skip((pageIndex - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            return bestseller;
        }
        public List<Product> SearchProducts(string query)
        {
            var results = Context.Products.Where(p => p.Name.Contains(query)).ToList();
            return results;
        }
        public int IsExistBrand(string brand)
        {
            var Brand = Context.Brands.FirstOrDefault(b => b.BrandName == brand);
            if(Brand != null)
            {
                return Brand.BrandId;
            }
            else
            {
                return 0;
            }
        }
        public List<Product> SearchBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                return new List<Product>();

            return Context.Products.Where(p => p.Brand.BrandName.ToLower().Contains(brand.ToLower())).ToList();
        
        }



        public async Task<List<Product>>  GetRecentlyViewedProductsAsync(string userId, int pageIndex = 1, int pageSize = 10)
        {
            return await Context.RecentlyViewedProducts.Where(rv => rv.UserId == userId).OrderByDescending(rv => rv.ViewedAt)
                                      .Skip((pageIndex - 1) * pageSize)  
                                      .Take(pageSize)
                                      .Select(rv => rv.Product)
                                      .ToListAsync();
        }
        public async Task AddRecentlyViewedProductAsync(RecentlyViewedProduct recentlyViewedProduct)
        {
            var existingProduct = await Context.RecentlyViewedProducts.FirstOrDefaultAsync(rv => rv.UserId == recentlyViewedProduct.UserId && rv.ProductId == recentlyViewedProduct.ProductId);

            if (existingProduct == null)
            {
                Context.RecentlyViewedProducts.Add(recentlyViewedProduct);
                await Context.SaveChangesAsync();
            }
        }
        public int GetRecentlyViewedCount(string userId)
        {
            return  Context.RecentlyViewedProducts.Count(rv => rv.UserId == userId);
        }
        public async Task<List<Product>> Get6RecentlyViewedProductsAsync(string userId)
        {
            var recentlyViewed = await Context.RecentlyViewedProducts
     .Where(rv => rv.UserId == userId) 
     .OrderByDescending(rv => rv.ViewedAt) 
     .Take(6) 
     .Select(rv => rv.Product)
     .ToListAsync(); 

            return recentlyViewed;
        }
    }
}
