using JumiaProject.Context;
using JumiaProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using JumiaProject.Interfaces;
namespace JumiaProject.Repositories
{
    public class CartRepo:ICart
    {
        private readonly JumiaContext context;
        private readonly UserManager<ApplicationUser> userManager;
        public CartRepo(JumiaContext _context,UserManager<ApplicationUser> userManager) {
        this.context = _context;
        this.userManager = userManager;
        }
        public async Task<Cart> GetCartByUserId(string userId)
        {
            var cart = await this.context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(c => c.Product)
                .ThenInclude(ci => ci.ProductVariants)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart() { UserId = userId , CartItems = new List<CartItem>() };
                await context.Carts.AddAsync(cart);           
                await context.SaveChangesAsync();                 
            }

            return cart;
        }
        public async Task<bool> AddItemToCart(string userId, CartItem cartItem)
        {
            var cart=await GetCartByUserId(userId);
            cart.CartItems.Add(cartItem);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveItemFromCart(string userId, int itemId)
        {
            var cart =await GetCartByUserId(userId);
            var item=cart.CartItems.FirstOrDefault(d=>d.CartItemId == itemId);
            if(item != null)
            {
                context.CartItems.Remove(item);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<decimal> CalculateCartTotalPrice(string userId)
        {
            var cart = await GetCartByUserId(userId);
            decimal price=cart?.CartItems.Sum(item=> (item.Product.Price - (item.Product.Price * @item.Product.Discount) )* item.Quantity)??0;
            return price;
        }

        public async Task<List<CartItem>> GetAllCartItems(string userId)
        {
            //var cart=await GetCartByUserId(userId);
            //var myItems = cart.CartItems.ToList();
            //return myItems ?? new List<CartItem>();
            var cart = await GetCartByUserId(userId);

            if (cart == null || cart.CartItems == null)
                return new List<CartItem>();

            return cart.CartItems.ToList();
        }

        public async Task<bool> UpdateQuantityInCart(string userId,CartItem cartItem)
        {
            var cart = await GetCartByUserId(userId);
            var existItem = cart.CartItems.FirstOrDefault(c=>c.CartItemId==cartItem.CartItemId);
            if (existItem==null) { 
            return false;
            }
            existItem.Quantity=cartItem.Quantity;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task AddOrUpdateCartItem(int cartId, int productId, int? variantId, int quantity)
        {
            var existingItem = await context.CartItems.FirstOrDefaultAsync(ci =>
                ci.CartId == cartId &&
                ci.ProductId == productId &&
               ( ci.VariantId == variantId || (ci.VariantId == null && variantId == null)));

            int availableStock = 0;
            decimal price = 0;

            if (variantId.HasValue)
            {
                var variantInfo = await context.ProductVariants
                    .Where(v => v.VariantId == variantId.Value)
                    .Select(v => new { v.Stock, v.Product.Price })
                    .FirstOrDefaultAsync();

                if (variantInfo != null)
                {
                    availableStock = variantInfo.Stock;
                    price = variantInfo.Price;
                }
            }
            else
            {
                var productInfo = await context.Products
                    .Where(p => p.ProductId == productId)
                    .Select(p => new { p.Stock, p.Price })
                    .FirstOrDefaultAsync();

                if (productInfo != null)
                {
                    availableStock = productInfo.Stock;
                    price = productInfo.Price;
                }
            }

            if (existingItem != null)
            {
                if (quantity <= 0)
                {
                    context.CartItems.Remove(existingItem);
                }
                else
                {
                    // ✅ هنا التعديل: بنزود على الكمية الحالية
                    var newQuantity =  quantity;
                    existingItem.Quantity = Math.Min(newQuantity, availableStock);
                    context.CartItems.Update(existingItem);
          
                }
            }
            else if (quantity > 0 && availableStock > 0)
            {
                var finalQuantity = Math.Min(quantity, availableStock);

                var newItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    VariantId = variantId,
                    Quantity = finalQuantity,
                    PriceAtTime = price
                };

                await context.CartItems.AddAsync(newItem);
            }

            await context.SaveChangesAsync();
        }

        public async Task<int?> GetAllCartItemsQuantity(string userId)
        {
            var cart = await GetCartByUserId(userId);
            int? quantity = cart?.CartItems.Sum(item => (item.Quantity ));
            return quantity;
        }
        public async Task<int> GetCartItemQuantity(int cartId, int productId, int? variantId)
        {
            var cartItem = await context.CartItems.FirstOrDefaultAsync(ci =>
                ci.CartId == cartId &&
                ci.ProductId == productId &&
                  (ci.VariantId == variantId || (ci.VariantId == null && variantId == null)));

            return cartItem?.Quantity??0 ;
        }
        public async Task<int> GetTotalCartQuantity(int cartId)
        {
            return await context.CartItems
                .Where(ci => ci.CartId == cartId)
                .SumAsync(ci => (int?)ci.Quantity) ?? 0;
        }
    }
}
