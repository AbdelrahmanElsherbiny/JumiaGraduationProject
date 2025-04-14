using JumiaProject.Models;

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
        public List<Product> GetProductsByCategory(string category);
        public bool AddProduct(Product product);
        public bool UpdateProduct(Product product);
        public void DeleteProduct(int id);
        List<Product> SearchProducts(string searchTerm, string statusFilter, int pageNum);
        int GetFilteredProductsCount(string searchTerm, string statusFilter);
    }
}
