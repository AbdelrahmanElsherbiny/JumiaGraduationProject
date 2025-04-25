using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

namespace JumiaProject.Repositories
{
    public class OrderItemRepo:IOrderItem
    {
        JumiaContext Context;
        public OrderItemRepo(JumiaContext context)
        {
            Context = context;
        }
        public void AddOrderItem(OrderItem orderItem)
        {
            Context.OrderItems.Add(orderItem);
            Context.SaveChanges();
        }
    }
}
