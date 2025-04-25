using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class WishlistRepo:IWishlist
    {
        private readonly JumiaContext Context;
       
        public WishlistRepo(JumiaContext _context)
        {
            Context = _context;
          
        }
        public List<Wishlist> GetWishlist(string userId)
        {
            var wishlist = Context.Wishlists
                .Where(w => w.UserId == userId)
                .ToList();
            return wishlist;
        }
        public void AddToWishlist(Wishlist wishlist)
        {
            Context.Wishlists.Add(wishlist);
            Context.SaveChanges();
        }
        public void RemoveFromWishlist(int WishlistId)
        {
            var wishlist = Context.Wishlists.Find(WishlistId);
            if (wishlist != null)
            {
                Context.Wishlists.Remove(wishlist);
                Context.SaveChanges();
            }
        }
        public bool ExistsInWishlist(string userId, int productId, int? productVariantId)
        {
            return productVariantId.HasValue
        ? Context.Wishlists.Any(w => w.UserId == userId && w.ProductId == productId && w.ProductVariantId == productVariantId.Value)
        : Context.Wishlists.Any(w => w.UserId == userId && w.ProductId == productId && w.ProductVariantId == null);
        }
        public Wishlist GetWishlistItem(string userId, int productId, int? productVariantId)
        {
           
            return Context.Wishlists.FirstOrDefault(w => w.UserId == userId && w.ProductId == productId && w.ProductVariantId == productVariantId);
        }
    }
}
