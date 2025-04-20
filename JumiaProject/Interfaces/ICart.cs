
using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface ICart
    {
        public Task<Cart> GetCartByUserId (string useId);
        public Task<bool> AddItemToCart(string userId,CartItem cartItem);
        public Task<bool> RemoveItemFromCart(string userId,int itemId);
        public Task<decimal> CalculateCartTotalPrice(string userId);
        public Task<List<CartItem>> GetAllCartItems(string userId);
        public Task<bool> UpdateQuantityInCart(string userId, CartItem cartItem);
        public Task AddOrUpdateCartItem(int cartId, int productId, int? variantId, int quantity);
        public Task<int?> GetAllCartItemsQuantity(string userId);
        public Task<int> GetCartItemQuantity(int cartId, int productId, int? variantId);
        public Task<int> GetTotalCartQuantity(int cartId);
    }
}
