using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface ISeller
    {
        public List<ApplicationUser> GetAllVerifiedSellers();
        public List<ApplicationUser> GetVerifiedSellersPaginated(int Page);
        public List<ApplicationUser> GetAllUnVerifiedSellers();
        public List<ApplicationUser> GetUnVerifiedSellersPaginated(int Page);
        public ApplicationUser GetSellerById(string id);
        public bool VerifySeller(ApplicationUser seller);
    }
}
