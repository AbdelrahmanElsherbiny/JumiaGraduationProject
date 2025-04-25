using JumiaProject.Context;
using JumiaProject.Interfaces;

namespace JumiaProject.Repositories
{
    public class CartItemRepo:ICartItem
    {
        JumiaContext Context;
        public CartItemRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public void RemoveCartItem(int cartItemId)
        {
            var cartItem = Context.CartItems.FirstOrDefault(x => x.CartItemId == cartItemId);
            if (cartItem != null)
            {
                Context.CartItems.Remove(cartItem);
                Context.SaveChanges();
            }
        }
    }
}
