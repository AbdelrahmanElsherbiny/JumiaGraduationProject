using System.ComponentModel.DataAnnotations;

namespace JumiaProject.ViewModels
{
    public class UserVM
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Username must contain only letters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone number must contain only numbers")]
        public string PhoneNumber { get; set; }
    }
}
