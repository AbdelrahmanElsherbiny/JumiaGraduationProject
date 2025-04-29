using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

namespace JumiaProject.Repositories
{
    public class AddressRepo:IAddress
    {
        JumiaContext Context;
        public AddressRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public List<Address> GetUserAddresses(string id)
        {
            return Context.Addresses.Where(a => a.UserId == id).ToList();
        }
        public Address GetAddressById(int id)
        {
            return Context.Addresses.FirstOrDefault(a => a.AddressId == id);
        }
        public void UpdateAddress(Address address)
        {
            Context.Addresses.Update(address);
            Context.SaveChanges();
        }
        public void AddAddress(Address address)
        {
            Context.Addresses.Add(address);
            Context.SaveChanges();
        }
    }
}
