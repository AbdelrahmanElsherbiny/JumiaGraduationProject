using JumiaProject.Models;
using JumiaProject.Repositories;

namespace JumiaProject.ViewModels
{
    public class ProfileVM
    {
        public UserVM User { get; set; }
        public AddressVM Address { get; set; }
    }
}
