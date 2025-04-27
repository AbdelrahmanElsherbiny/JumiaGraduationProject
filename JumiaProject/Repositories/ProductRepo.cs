using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JumiaProject.Repositories
{
    public class ProductRepo : IProduct
    {
        private readonly JumiaContext _context;
        private readonly ILogger<ProductRepo> _logger;

        public ProductRepo(JumiaContext context, ILogger<ProductRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.Where(p => p.IsApprovedFromAdmin != "Not Approved" && p.IsDeleted == false).ToList();
        }

        public (List<Product> Products, int TotalCount) GetProductsPaginated(int page, int pageSize, string search, string filters)
        {
            var query = _context.Products.Include(p => p.ProductImages).Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
            }

            // Apply filters (e.g., price, brand, discount, stock, units sold, visibility)
            // Logic for filters goes here

            var totalCount = query.Count();
            var products = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return (products, totalCount);
        }

        public void VerifyProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            product.IsApprovedFromAdmin = "Approved";
            _context.SaveChanges();
        }

        public void UnVerifyProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            product.IsApprovedFromAdmin = "Not Approved";
            _context.SaveChanges();
        }

        public Product GetProductByName(string name)
        {
            return _context.Products.FirstOrDefault(p => p.Name == name);
        }

        public Product GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsDeleted == false)
                .FirstOrDefault(p => p.ProductId == id);
        }


        public void DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            product.IsDeleted = true;
            _context.SaveChanges();
        }

        public bool UpdateProduct(Product updatedProduct)
        {
            try
            {
                _logger.LogInformation("Starting product update for ProductId: {ProductId}", updatedProduct.ProductId);

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var existingProduct = _context.Products
                            .Include(p => p.ProductImages)
                            .FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);

                        if (existingProduct == null)
                        {
                            _logger.LogError("Product not found for update. ProductId: {ProductId}", updatedProduct.ProductId);
                            return false;
                        }

                        // Update basic product properties
                        existingProduct.Name = updatedProduct.Name;
                        existingProduct.Description = updatedProduct.Description;
                        existingProduct.Price = updatedProduct.Price;
                        existingProduct.Discount = updatedProduct.Discount;
                        existingProduct.Stock = updatedProduct.Stock;
                        existingProduct.CategoryId = updatedProduct.CategoryId;
                        existingProduct.BrandId = updatedProduct.BrandId;
                        existingProduct.MainMaterial = updatedProduct.MainMaterial;
                        existingProduct.SKU = updatedProduct.SKU;

                        // Handle product images if they exist
                        if (updatedProduct.ProductImages != null && updatedProduct.ProductImages.Any())
                        {
                            // Create a separate list to avoid collection modification during enumeration
                            var imagesToRemove = existingProduct.ProductImages.ToList();
                            foreach (var image in imagesToRemove)
                            {
                                _context.ProductImages.Remove(image);
                            }

                            foreach (var newImage in updatedProduct.ProductImages)
                            {
                                existingProduct.ProductImages.Add(new ProductImage
                                {
                                    ImageUrl = newImage.ImageUrl,
                                    IsPrimary = newImage.IsPrimary
                                });
                            }
                        }

                        _context.SaveChanges();
                        transaction.Commit();
                        _logger.LogInformation("Product update completed successfully. ProductId: {ProductId}", updatedProduct.ProductId);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Error updating product. ProductId: {ProductId}", updatedProduct.ProductId);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateProduct method. ProductId: {ProductId}", updatedProduct.ProductId);
                return false;
            }
        }

        public List<Product> GetProductsByCategory(int id)
        {
            return _context.Products.Where(p => p.Category.CategoryId == id).ToList();
        }
        public List<Product> GetProductsByBrand(int id, int pageIndex = 1, int pageSize = 10)
        {
            return Context.Products.Where(p => p.BrandId == id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        public List<Product> SearchProducts(string searchTerm, string statusFilter, int pageNum)
        {
            var query = _context.Products.Where(p => p.IsDeleted == false && p.IsApprovedFromAdmin != "Not Approved").AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()) && p.IsDeleted == false);
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
            var query = _context.Products.Where(p => p.IsDeleted == false && p.IsApprovedFromAdmin != "Not Approved").AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()) && p.IsDeleted == false);
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

        public List<Product> GetAllProductsForSeller(string? sellerId)
        {
            return _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.SellerId == sellerId && p.IsDeleted == false)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }

        public List<Product> GetProductsPaginated(int pageNumber = 1)
        {
            throw new NotImplementedException();
        }

        public void AddProduct(Product product)
        {
            try
            {
                Console.WriteLine("Starting AddProduct in ProductRepo...");
                Console.WriteLine($"Product details:");
                Console.WriteLine($"- Name: {product.Name}");
                Console.WriteLine($"- CategoryId: {product.CategoryId}");
                Console.WriteLine($"- BrandId: {product.BrandId}");
                Console.WriteLine($"- SellerId: {product.SellerId}");
                Console.WriteLine($"- Number of images: {product.ProductImages?.Count ?? 0}");

                // Begin transaction
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Console.WriteLine("Beginning database transaction...");

                        // Add the product
                        Console.WriteLine("Adding product to context...");
                        _context.Products.Add(product);
                        
                        var saveResult = _context.SaveChanges();
                        Console.WriteLine($"SaveChanges result (product): {saveResult}");
                        Console.WriteLine($"Generated ProductId: {product.ProductId}");

                        // Add the images
                        if (product.ProductImages != null && product.ProductImages.Any())
                        {
                            // Create a separate list of images to process
                            var imagesToProcess = product.ProductImages.ToList();
                            Console.WriteLine($"Processing {imagesToProcess.Count} images...");
                            
                            foreach (var image in imagesToProcess)
                            {
                                _context.ProductImages.Add(new ProductImage
                                {
                                    ProductId = product.ProductId,
                                    ImageUrl = image.ImageUrl,
                                    IsPrimary = image.IsPrimary
                                });
                                Console.WriteLine($"Added image: {image.ImageUrl}");
                            }
                            
                            saveResult = _context.SaveChanges();
                            Console.WriteLine($"SaveChanges result (images): {saveResult}");
                        }

                        // Commit transaction
                        Console.WriteLine("Committing transaction...");
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully");
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction on error
                        Console.WriteLine("Error occurred, rolling back transaction...");
                        Console.WriteLine($"Error details: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        transaction.Rollback();
                        throw new Exception($"Error saving product: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddProduct: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw new Exception($"Error in AddProduct: {ex.Message}", ex);
            }
        }
    }
}
