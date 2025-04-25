using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using Microsoft.EntityFrameworkCore;

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
        public List<Order> GetOrdersPaginated(int pageNum)
        {
            int pageSize = 10;
            int skip = (pageNum - 1) * pageSize;
            return Context.Orders.Skip(skip).Take(pageSize).ToList();
        }
        public List<Order> GetOrdersByUserId(string userId)
        {
            return Context.Orders.Where(x => x.UserId == userId).ToList();
        }
        public List<Order> SearchOrders(string searchTerm, string statusFilter, int pageNum)
        {
            var query = Context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(o =>
                    o.OrderId.ToString().Contains(searchTerm) ||
                    o.User.UserName.ToLower().Contains(searchTerm.ToLower()));
            }

            if (statusFilter != "all")
            {
                query = query.Where(o => o.OrderStatus == statusFilter);
            }

            return query
                .OrderBy(o => o.OrderId)
                .Skip((pageNum - 1) * 10)
                .Take(10)
                .ToList();
        }

        public int GetFilteredOrdersCount(string searchTerm, string statusFilter)
        {
            var query = Context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(o =>
                    o.OrderId.ToString().Contains(searchTerm) ||
                    o.User.UserName.ToLower().Contains(searchTerm.ToLower()));
            }

            if (statusFilter != "all")
            {
                query = query.Where(o => o.OrderStatus == statusFilter);
            }

            return query.Count();
        }

        public List<Order> GetOrdersForSeller(string sellerId)
        {
            // Fetch orders that include products from the specified seller
            var orders = Context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == sellerId))
                .ToList();

            // Filter the order items to include only the products of the specified seller
            foreach (var order in orders)
            {
                order.OrderItems = order.OrderItems
                    .Where(oi => oi.Product.SellerId == sellerId)
                    .ToList();
            }

            return orders;
        }
    }
}
