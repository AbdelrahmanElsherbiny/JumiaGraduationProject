using JumiaProject.Models;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Interfaces
{
    public interface IWishlist
    {
        public List<Wishlist> GetWishlist(string userId);
        public void AddToWishlist(Wishlist wishlist);
        public void RemoveFromWishlist(int wishlistId);
        public bool ExistsInWishlist(string userId, int productId, int? productVariantId);
        public Wishlist GetWishlistItem(string userId, int productId, int? productVariantId);
    }
}
