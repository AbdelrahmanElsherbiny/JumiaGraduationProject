using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

namespace JumiaProject.Repositories
{
    public class PaymentRepo:IPayment
    {
        JumiaContext Context;
        public PaymentRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public void AddPayment(Payment payment)
        {
            Context.Payments.Add(payment);
            Context.SaveChanges();
        }
        public Payment GetPaymentByOrderId(int orderId)
        {
            return Context.Payments.FirstOrDefault(p => p.OrderId == orderId);
        }
        public void UpdatePayment(Payment payment)
        {
            Context.Payments.Update(payment);
            Context.SaveChanges();
        }
    }
}
