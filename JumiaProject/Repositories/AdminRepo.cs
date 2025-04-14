using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.AspNetCore.Identity;

namespace JumiaProject.Repositories
{
    public class AdminRepo:IAdmin
    {
        JumiaContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        public AdminRepo(JumiaContext _context, UserManager<ApplicationUser> userManager)
        {
            Context = _context;
            UserManager = userManager;
        }
        public ApplicationUser GetAdminById(string id)
        {
            return Context.Users.FirstOrDefault(u => u.Id == id);
        }
        public async Task<ApplicationUser> GetMasterAdmin()
        {
            var users = await UserManager.GetUsersInRoleAsync("Admin");
            return users[0];
        }

    }
}
