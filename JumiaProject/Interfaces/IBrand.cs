using JumiaProject.Models;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Interfaces
{
    public interface IBrand
    {

        public List<Brand> GetAllBrands();

    }
}
