using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IUser
    {
        public Task<List<ApplicationUser>> GetAllCustomers();
        public Task<List<ApplicationUser>> GetCustomersPaginated(int page);
        public ApplicationUser GetUserById(string id);
        public bool AddUser(ApplicationUser user);
        public bool UpdateUser(ApplicationUser user);
        public bool DeleteUser(string id);
    }
}
