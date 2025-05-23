﻿using JumiaProject.Models;
using JumiaProject.ViewModels;

namespace JumiaProject.Interfaces
{
    public interface IProduct
    {
        public List<Product> GetAllProducts();
        public List<Product> GetProductsPaginated(int pageNumber = 1);
        public void VerifyProduct(int id);
        public void UnVerifyProduct(int id);
        public Product GetProductByName(string name);
        public Product GetProductById(int id);
        public List<Product> GetProductsByCategory(int id);
        public List<Product> GetProductsByBrand(int id, int pageIndex = 1, int pageSize = 10);
        public bool UpdateProduct(Product product);
        public void DeleteProduct(int id);
        List<Product> SearchProducts(string searchTerm, string statusFilter, int pageNum);
        int GetFilteredProductsCount(string searchTerm, string statusFilter);
        List<Product> GetAllProductsForSeller(string? name);
        (List<Product> Products, int TotalCount) GetProductsPaginated(int page, int pageSize, string search, string filters);
        void AddProduct(Product product);
        public List<Product> Get6BestSeller();
        public List<Product> GetBestSeller(int pageIndex = 1, int pageSize = 10);
        public List<Product> GetMostDiscount(int pageIndex = 1, int pageSize = 10);
        public int GetBestSellerCount();
        public int GetMostDiscountCount();
        public int GetProductsByBrandCount(int id);
        List<Product> SearchProducts(string query);
        public int IsExistBrand(string brand);
        List<Product> SearchBrand(string brand);
        Task<List<Product>> GetTopSellingProductsAsync(int count = 10);
        Task<List<Product>> GetPendingApprovalProductsAsync(int count = 10);
        Task<int> GetApprovedProductsCountAsync();
        Task<Dictionary<string,int>> GetCategoryDistributionAsync();



        Task<List<Product>> GetRecentlyViewedProductsAsync(string userId, int pageIndex = 1, int pageSize = 10);
        Task AddRecentlyViewedProductAsync(RecentlyViewedProduct recentlyViewedProduct);
        public int GetRecentlyViewedCount(string userId);
        Task<List<Product>> Get6RecentlyViewedProductsAsync(string userId);
    }
}
   
