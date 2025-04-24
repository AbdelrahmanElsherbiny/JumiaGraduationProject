using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

namespace JumiaProject.Repositories
{
    public class ProductVariantRepo:IProductVariant
    {
        JumiaContext Context;
        public ProductVariantRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public ProductVariant GetProductVariantById(int? id)
        {
            var productVariant = Context.ProductVariants.FirstOrDefault(x => x.VariantId == id);
            if (productVariant == null)
            {
                return null;
            }
            return productVariant;
        }
        public void UpdateProductVariant(ProductVariant productVariant)
        {
            var existingProductVariant = Context.ProductVariants.FirstOrDefault(x => x.VariantId == productVariant.VariantId);
            Context.Update(productVariant);
            Context.SaveChanges();
        }
    }
}
