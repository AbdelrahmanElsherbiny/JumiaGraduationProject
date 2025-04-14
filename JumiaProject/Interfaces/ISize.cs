using JumiaProject.Models;

namespace JumiaProject.Interfaces
{
    public interface ISize
    {
        public List<Size> GetAllSizes();
        public void AddSize(Size size);
        public void AddCategorySize(CategorySize categorySize);
        public bool SizeExists(string sizeLabel);
    }
}
