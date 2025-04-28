using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface IAddress
    {
        public List<Address> GetUserAddresses(string id);
        public Address GetAddressById(int id);
        public void UpdateAddress(Address address);
        public void AddAddress(Address address);

    }
}
