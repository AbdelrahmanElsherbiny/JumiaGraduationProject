using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IPayment
    {
        public void AddPayment(Payment payment);
        public Payment GetPaymentByOrderId(int orderId);
        public void UpdatePayment(Payment payment);
    }
}
