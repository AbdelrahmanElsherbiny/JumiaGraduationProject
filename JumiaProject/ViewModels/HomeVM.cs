using JumiaProject.Models;
using JumiaProject.Repositories;

namespace JumiaProject.ViewModels
{
    public class HomeVM
    {
        public List<Slider> SliderItems { get; set; }
        public List<ProductVM> Products { get; set; }
        public List<CategoryVM> Categories { get; set; }
    }
}
