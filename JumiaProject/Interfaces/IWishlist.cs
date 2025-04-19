using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IWishlist
    {
        public List<Wishlist> GetWishlist(string userId);
        public void AddToWishlist(Wishlist wishlist);
        public void RemoveFromWishlist(int wishlistId);
    }
}
