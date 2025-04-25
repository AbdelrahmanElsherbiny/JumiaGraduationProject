using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface ISeller
    {
        public Task<List<ApplicationUser>> GetAllSellers();
        public Task<List<ApplicationUser>> GetAllSellersPaginated(int pageNum);
        public List<ApplicationUser> GetAllVerifiedSellers();
        public void BlockSeller(string id);
        public void Verify(string id);
        public void UnVerify(string id);
        public List<ApplicationUser> GetVerifiedSellersPaginated(int PageNum);
        public List<ApplicationUser> GetAllUnVerifiedSellers();
        public List<ApplicationUser> GetUnVerifiedSellersPaginated(int PageNum);
        public ApplicationUser GetSellerById(string id);
        public bool VerifySeller(ApplicationUser seller);
        public Task<List<ApplicationUser>> SearchSellers(string searchTerm, int pageNum);
        public Task<int> GetFilteredSellersCount(string searchTerm);
        public bool UpdateSeller(ApplicationUser user);
    }
}
