using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class SellerRepo : ISeller
    {
        JumiaContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        public SellerRepo(JumiaContext _context,UserManager<ApplicationUser> _userManager)
        {
            Context = _context;
            UserManager = _userManager;
        }
        public async Task<List<ApplicationUser>> GetAllSellers()
        {
            var users = await UserManager.GetUsersInRoleAsync("Seller");
            return users.Where(u => u.IsDeleted == false && u.Seller.IsVerified!=false).ToList();
        }
        public void BlockSeller(string id)
        {
            var user = GetSellerById(id);
            if (user != null)
            {
                user.Seller.IsVerified = false;
                Context.SaveChanges();
            }
        }
        public void Verify(string id)
        {
            var user = GetSellerById(id);
            if (user != null)
            {
                user.Seller.IsVerified = true;
                Context.SaveChanges();
            }
        }
        public void UnVerify(string id)
        {
            var user = GetSellerById(id);
            if (user != null)
            {
                user.Seller.IsVerified = false;
                Context.SaveChanges();
            }
        }
        public async Task<List<ApplicationUser>> GetAllSellersPaginated(int pageNum)
        {
            int pageSize = 10;
            int skip = (pageNum - 1) * pageSize;
            var users = await UserManager.GetUsersInRoleAsync("Seller");
            return users.Where(u => u.IsDeleted == false && u.Seller.IsVerified != false).OrderBy(u=>u.Seller.IsVerified).Skip(skip).Take(pageSize).ToList();
        }
        public List<ApplicationUser> GetAllVerifiedSellers()
        {
            return Context.Users.Where(x => x.Seller.IsVerified == true).ToList();
        }
        public List<ApplicationUser> GetAllUnVerifiedSellers()
        {
            return Context.Users.Where(x => x.Seller.IsVerified == null).ToList();
        }
        public ApplicationUser GetSellerById(string id)
        {
            return Context.Users.FirstOrDefault(x => x.Id == id);
        }
        public bool VerifySeller(ApplicationUser seller)
        {
            var user = Context.Users.FirstOrDefault(x => x.Id == seller.Id);
            if (user != null)
            {
                user.Seller.IsVerified = true;
                Context.SaveChanges();
                return true;
            }
            return false;
        }
        public List<ApplicationUser> GetVerifiedSellersPaginated(int PageNum)
        {
            int pageSize = 10;
            int skip = (PageNum - 1) * pageSize;
            return Context.Users.Where(x => x.Seller.IsVerified == true).Skip(skip).Take(pageSize).ToList();
        }
        public List<ApplicationUser> GetUnVerifiedSellersPaginated(int PageNum)
        {
            int pageSize = 10;
            int skip = (PageNum - 1) * pageSize;
            return Context.Users.Where(x => x.Seller.IsVerified == false).Skip(skip).Take(pageSize).ToList();
        }
        public async Task<List<ApplicationUser>> SearchSellers(string searchTerm, int pageNum)
        {
            var query = GetAllSellers().Result.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(searchTerm.ToLower()) ||
                    u.Email.ToLower().Contains(searchTerm.ToLower()) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            return query
                .OrderBy(u => u.Seller.IsVerified)
                .ThenBy(u => u.Seller.SellerId)
                .ThenBy(u => u.UserName)
                .Skip((pageNum - 1) * 10)
                .Take(10)
                .ToList();
        }

        public async Task<int> GetFilteredSellersCount(string searchTerm)
        {
            var query = GetAllSellers().Result.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(searchTerm.ToLower()) ||
                    u.Email.ToLower().Contains(searchTerm.ToLower()) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            return query.Count();
        }

        public bool UpdateSeller(ApplicationUser user)
        {
            try
            {
                // Find the user without tracking first
                var trackedUser = Context.Users
                    .Include(u => u.Seller)
                    .FirstOrDefault(u => u.Id == user.Id);

                if (trackedUser == null)
                {
                    return false;
                }

                // Update only the specific properties
                trackedUser.Email = user.Email;
                trackedUser.PhoneNumber = user.PhoneNumber;

                if (trackedUser.Seller != null)
                {
                    trackedUser.Seller.StoreName = user.Seller.StoreName;
                    trackedUser.Seller.TaxNumber = user.Seller.TaxNumber;
                    trackedUser.Seller.BankAccount = user.Seller.BankAccount;
                }

                // Save changes and return success based on rows affected
                int result = Context.SaveChanges();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateSeller: {ex.Message}");
                return false;
            }
        }
    }
}
