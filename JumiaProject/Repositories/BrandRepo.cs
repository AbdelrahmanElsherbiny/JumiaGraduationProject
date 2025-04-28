using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;

namespace JumiaProject.Repositories
{
    public class BrandRepo : IBrand
    {
        private readonly JumiaContext _context;

        public BrandRepo(JumiaContext context)
        {
            _context = context;
        }

        public List<Brand> GetAllBrands()
        {
            return _context.Brands.ToList();
        }
    }
}