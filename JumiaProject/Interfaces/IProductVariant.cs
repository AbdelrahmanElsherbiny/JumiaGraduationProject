using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IProductVariant
    {
        public ProductVariant GetProductVariantById(int? id);
        public void UpdateProductVariant(ProductVariant productVariant);
    }
}
