using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IUser
    {
        public Task<ApplicationUser> GetUserById(string id);
        public Task<List<ApplicationUser>> GetAllCustomers();
        public Task<List<ApplicationUser>> GetCustomersPaginated(int page);
        public bool AddUser(ApplicationUser user);
        public bool UpdateUser(ApplicationUser user);
        public bool DeleteUser(string id);
        public Task<List<ApplicationUser>> SearchCustomers(string searchTerm, int pageNum);
        public Task<int> GetFilteredCustomersCount(string searchTerm);
    }
}
