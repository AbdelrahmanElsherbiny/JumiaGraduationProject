using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.AspNetCore.Identity;

namespace JumiaProject.Repositories
{
    public class UserRepo:IUser
    {
        JumiaContext Context;
        private readonly UserManager<ApplicationUser> UserManager;

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
        public async Task<List<ApplicationUser>> GetCustomersPaginated(int page)
        {
            int pageSize = 10;
            int skip = (page - 1) * pageSize;
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
            try
            {
                Context.Users.Add(user);
                Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateUser(ApplicationUser user)
        {
            try
            {
                Context.Users.Update(user);
                Context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteUser(string id)
        {
            try
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
            catch (Exception)
            {
                return false;
            }
        }

    }
}
