using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class UserDeleteAccRepo : IUserDeleteAcc
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly JumiaContext Context;
        public UserDeleteAccRepo(UserManager<ApplicationUser> userManager, JumiaContext Context)
        {
            this.userManager = userManager;
            this.Context = Context;
        }
        public async Task<bool> DeleteAccount(string email)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null || user.IsDeleted)
                {
                    return false;
                }

                user.IsDeleted = true;
                var result = await userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> ValidatePassword(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email); 
            if (user == null)
            {
                return false;
            }
            var result = await userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task RemoveUserDataByEmail(string email)
        {
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return; 
            }
            var wishlistItems = await Context.Wishlists.Where(w => w.UserId == user.Id).ToListAsync();
            Context.Wishlists.RemoveRange(wishlistItems);
            var orders = await Context.Orders.Where(o => o.UserId == user.Id).ToListAsync();
            Context.Orders.RemoveRange(orders);
            var recentlyViewed = await Context.RecentlyViewedProducts.Where(rv => rv.UserId == user.Id).ToListAsync();
            Context.RecentlyViewedProducts.RemoveRange(recentlyViewed);
            await Context.SaveChangesAsync();
        }
    }
}
