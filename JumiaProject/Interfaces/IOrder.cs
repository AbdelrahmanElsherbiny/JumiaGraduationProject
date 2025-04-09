using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IOrder
    {
        public Order GetOrderById(int id);
        public List<Order> GetAllOrders();
        public List<Order> GetOrdersPaginated(int page);
        public List<Order> GetOrdersByUserId(string userId);
    }
}
