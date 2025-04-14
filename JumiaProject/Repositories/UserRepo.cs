using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class UserRepo:IUser
    {
        JumiaContext Context;
        UserManager<ApplicationUser> UserManager;

        public UserRepo(JumiaContext context, UserManager<ApplicationUser> _userManager)
        {
            Context = context;
            UserManager = _userManager;
        }
        public async Task<List<ApplicationUser>> GetAllCustomers()
        {
            var users = await UserManager.GetUsersInRoleAsync("Customer");
            return users.Where(u => u.IsDeleted == false).ToList();
        }
        public async Task<List<ApplicationUser>> GetCustomersPaginated(int pageNum)
        {
            int pageSize = 10;
            int skip = (pageNum - 1) * pageSize;
            var users = await UserManager.GetUsersInRoleAsync("Customer");
            return users.Where(u => u.IsDeleted == false).Skip(skip).Take(pageSize).ToList();
        }
        public ApplicationUser GetUserById(string id)
        {
            var user = Context.Users.FirstOrDefault(u => u.Id == id);
            return user;
        }
        public bool AddUser(ApplicationUser user)
        {
            Context.Users.Add(user);
            Context.SaveChanges();
            return true;
        }
        public bool UpdateUser(ApplicationUser user)
        {
            Context.Users.Update(user);
            Context.SaveChanges();
            return true;
        }
        public bool DeleteUser(string id)
        {
            var user = Context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.IsDeleted = true;
                Context.Users.Update(user);
                Context.SaveChanges();
                return true;
            }
            return false;
        }
        public async Task<List<ApplicationUser>> SearchCustomers(string searchTerm, int pageNum)
        {
            var query = GetAllCustomers().Result.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(searchTerm.ToLower()) ||
                    u.Email.ToLower().Contains(searchTerm.ToLower()) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            return query
                .OrderBy(u => u.Id)
                .Skip((pageNum - 1) * 10)
                .Take(10)
                .ToList();
        }

        public async Task<int> GetFilteredCustomersCount(string searchTerm)
        {
            var query = GetAllCustomers().Result.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(searchTerm.ToLower()) ||
                    u.Email.ToLower().Contains(searchTerm.ToLower()) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            return query.Count();
        }


    }
}
