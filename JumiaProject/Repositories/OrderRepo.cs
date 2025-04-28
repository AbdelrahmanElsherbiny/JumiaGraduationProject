using System.Globalization;
using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;
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
        public void AddOrder(Order order)
        {
            Context.Orders.Add(order);
            Context.SaveChanges();
        }
        public ShippingMethod GetShippingMethodById(int shippingMethodId)
        {
            return Context.ShippingMethods.FirstOrDefault(x => x.ShippingMethodId == shippingMethodId);
        }
        public async Task<List<Order>> GetRecentOrdersAsync(int count = 5)
        {
            return await Context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await Context.Orders
                .Where(o => o.OrderStatus != "Cancelled" && o.OrderStatus != "Returned")
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<Dictionary<string, decimal>> GetMonthlySalesDataAsync(int year)
        {
            // Create a dictionary with all months initialized to zero
            var monthlySales = new Dictionary<string, decimal>
    {
        { "Jan", 0 }, { "Feb", 0 }, { "Mar", 0 },
        { "Apr", 0 }, { "May", 0 }, { "Jun", 0 },
        { "Jul", 0 }, { "Aug", 0 }, { "Sep", 0 },
        { "Oct", 0 }, { "Nov", 0 }, { "Dec", 0 }
    };

            // Get orders for the specified year
            var ordersInYear = await Context.Orders
                .Where(o => o.OrderDate.HasValue &&
                           o.OrderDate.Value.Year == year &&
                           o.OrderStatus != "Cancelled")
                .ToListAsync();

            // Calculate sales for each month
            foreach (var order in ordersInYear)
            {
                if (order.OrderDate.HasValue)
                {
                    var monthName = order.OrderDate.Value.ToString("MMM");
                    monthlySales[monthName] += order.TotalAmount;
                }
            }

            return monthlySales;
        }
        public async Task<Dictionary<int, Dictionary<string, decimal>>> GetMultiYearMonthlySalesDataAsync(int years)
        {
            var result = new Dictionary<int, Dictionary<string, decimal>>();
            var currentYear = DateTime.Now.Year;

            for (int i = 0; i < years; i++)
            {
                var year = currentYear - i;
                result[year] = await GetMonthlySalesDataAsync(year);
            }

            return result;
        }

    }
}
