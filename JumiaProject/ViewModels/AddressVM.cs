using System.ComponentModel.DataAnnotations;

namespace JumiaProject.ViewModels
{
    public class AddressVM
    {
        public int AddressId { get; set; }
        [Required(ErrorMessage = "Country is required")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s\-]+$",
                ErrorMessage = "Country must contain only letters")]
        public string Country { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s\-]+$",
             ErrorMessage = "City must contain only letters")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF0-9\s\-\,\.]+$",
             ErrorMessage = "Address can contain letters, numbers, and basic punctuation")]
        public string Street { get; set; } = null!;
    }
}
