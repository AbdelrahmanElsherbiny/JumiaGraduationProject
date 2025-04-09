using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

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
            return Context.Products.ToList();
        }
        public List<Product> GetProductsPaginated(int pageNumber)
        {
            int pageSize = 10;
            return Context.Products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
        public Product GetProductByName(string name)
        {
            return Context.Products.FirstOrDefault(p => p.Name == name);
        }
        public Product GetProductById(int id)
        {
            return Context.Products.FirstOrDefault(p => p.ProductId == id);
        }
        public bool AddProduct(Product product)
        {
            try
            {
                Context.Products.Add(product);
                Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteProduct(int id)
        {
            try
            {
                var product = Context.Products.FirstOrDefault(p => p.ProductId == id);
                if (product != null)
                {
                    Context.Products.Remove(product);
                    Context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool UpdateProduct(Product product)
        {
            try
            {
                Context.Products.Update(product);
                Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<Product> GetProductsByCategory(string category)
        {
            return Context.Products.Where(p => p.Category.CategoryName == category).ToList();
        }
    }
}
