using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

namespace JumiaProject.Repositories
{
    public class SellerRepo:ISeller
    {
        JumiaContext Context;
        public SellerRepo(JumiaContext _context)
        {
            Context = _context;
        }
        public List<ApplicationUser> GetAllVerifiedSellers()
        {
            return Context.Users.Where(x => x.Seller.IsVerified == true).ToList();
        }
        public List<ApplicationUser> GetAllUnVerifiedSellers()
        {
            return Context.Users.Where(x => x.Seller.IsVerified == false).ToList();
        }
        public ApplicationUser GetSellerById(string id)
        {
            return Context.Users.FirstOrDefault(x => x.Id == id);
        }
        public bool VerifySeller(ApplicationUser seller)
        {
            var user = Context.Users.FirstOrDefault(x => x.Id == seller.Id);
            if (user != null)
            {
                user.Seller.IsVerified = true;
                Context.SaveChanges();
                return true;
            }
            return false;
        }
        public List<ApplicationUser> GetVerifiedSellersPaginated(int Page)
        {
            int pageSize = 10;
            int skip = (Page - 1) * pageSize;
            return Context.Users.Where(x => x.Seller.IsVerified == true).Skip(skip).Take(pageSize).ToList();
        }
        public List<ApplicationUser> GetUnVerifiedSellersPaginated(int Page)
        {
            int pageSize = 10;
            int skip = (Page - 1) * pageSize;
            return Context.Users.Where(x => x.Seller.IsVerified == false).Skip(skip).Take(pageSize).ToList();
        }
    }
}
