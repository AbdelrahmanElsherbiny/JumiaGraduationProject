using JumiaProject.Models;
using JumiaProject.ViewModels;

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
        public void AddOrder(Order order);
        ShippingMethod GetShippingMethodById(int shippingMethodId);
        Task<List<Order>> GetRecentOrdersAsync(int count = 5);
        Task<decimal> GetTotalRevenueAsync();
        Task<Dictionary<string,decimal>> GetMonthlySalesDataAsync(int year);
        Task<Dictionary<int, Dictionary<string, decimal>>> GetMultiYearMonthlySalesDataAsync(int years);
    }
}
