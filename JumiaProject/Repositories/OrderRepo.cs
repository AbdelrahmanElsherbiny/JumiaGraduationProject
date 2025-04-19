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
            return Context.Orders.Where(x => x.UserId == userId && x.OrderStatus!="Canceled" && x.OrderStatus != "Returned").ToList();
        }
        public List<Order> GetCanceledOrdersByUserId(string userId)
        {
            return Context.Orders.Where(x => x.UserId == userId && (x.OrderStatus == "Canceled" || x.OrderStatus == "Returned")).ToList();
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
    }
}
