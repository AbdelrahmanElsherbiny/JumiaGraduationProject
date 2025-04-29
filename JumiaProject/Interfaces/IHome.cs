using JumiaProject.Models;
using JumiaProject.ViewModels;

namespace JumiaProject.Interfaces
{
    public interface IHome
    {
        HomeVM GetData();
        HomeVM Search(string searchkey);
    }
}
