using JumiaProject.Models;
using JumiaProject.ViewModels;

namespace JumiaProject.Interfaces
{
    public interface IProfile
    {
        ProfileVM GetUserData(string id);
    }
}
