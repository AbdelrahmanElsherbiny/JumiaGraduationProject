using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IProduct
    {
        public List<Product> GetAllProducts();
        public List<Product> GetProductsPaginated(int pageNumber);
        public Product GetProductByName(string name);
        public Product GetProductById(int id);
        public List<Product> GetProductsByCategory(int id);
        public bool AddProduct(Product product);
        public bool UpdateProduct(Product product);
        public bool DeleteProduct(int id);
    }
}
