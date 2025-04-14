using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
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
        public ProductDetails GetProductById(int productId)
        {
            return Context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductDetails
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    Discount = p.Discount,
                    SKU = p.SKU, 
                    Code = p.Code
                })
                .FirstOrDefault();
        }

        Product IProduct.GetProductById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
