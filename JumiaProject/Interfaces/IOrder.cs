using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IOrder
    {
        public Order GetOrderById(int id);
        public List<Order> GetAllOrders();
        public List<Order> GetOrdersPaginated(int page);
        public List<Order> GetOrdersByUserId(string userId);
        public List<Order> GetCanceledOrdersByUserId(string userId);
        List<Order> SearchOrders(string searchTerm, string statusFilter, int pageNum);
        int GetFilteredOrdersCount(string searchTerm, string statusFilter);
    }
}
