using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IAdmin
    {
        public ApplicationUser GetAdminById(string id);
        public Task<ApplicationUser> GetMasterAdmin();
    }
}
