using System.ComponentModel.DataAnnotations;

namespace JumiaProject.ViewModels
{
    public class DeleteAccountVM
    {
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
