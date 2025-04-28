using JumiaProject.Models;
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
        public List<Product> GetProductsByBrand(int id);
        public bool AddProduct(Product product);
        public bool UpdateProduct(Product product);
        public void DeleteProduct(int id);
        List<Product> SearchProducts(string searchTerm, string statusFilter, int pageNum);
        int GetFilteredProductsCount(string searchTerm, string statusFilter);
        public List<Product> Get6BestSeller();
        public List<Product> GetBestSeller();
        public List<Product> GetMostDiscount();
        Task<List<Product>> GetTopSellingProductsAsync(int count = 10);
        Task<List<Product>> GetPendingApprovalProductsAsync(int count = 10);
        Task<int> GetApprovedProductsCountAsync();
        Task<Dictionary<string,int>> GetCategoryDistributionAsync();

    }
}
