using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

namespace JumiaProject.Repositories
{
    public class ShippingTrackingRepo:IShippingTracking
    {
        JumiaContext Context;
        public ShippingTrackingRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public void AddShippingTracking(ShippingTracking shippingTracking)
        {
            Context.ShippingTrackings.Add(shippingTracking);
            Context.SaveChanges();
        }
    }
}
