using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

namespace JumiaProject.Repositories
{
    public class OrderRepo:IOrder
    {
        JumiaContext Context;
        public OrderRepo(JumiaContext context)
        {
            Context = context;
        }
        public Order GetOrderById(int id)
        {
            return Context.Orders.FirstOrDefault(x => x.OrderId == id);
        }
        public List<Order> GetAllOrders()
        {
            return Context.Orders.ToList();
        }
        public List<Order> GetOrdersPaginated(int page)
        {
            int pageSize = 10;
            int skip = (page - 1) * pageSize;
            return Context.Orders.Skip(skip).Take(pageSize).ToList();
        }
        public List<Order> GetOrdersByUserId(string userId)
        {
            return Context.Orders.Where(x => x.UserId == userId).ToList();
        }
    }
}
