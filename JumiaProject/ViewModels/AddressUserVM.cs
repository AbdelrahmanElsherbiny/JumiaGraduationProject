using System.ComponentModel.DataAnnotations;

namespace JumiaProject.ViewModels
{
    public class AddressUserVM
    {
        public string UserId { get; set; } = null!;
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$", ErrorMessage = "Phone number must start with 010, 011, 012, or 015 and be 11 digits long.")]
        public string PhoneNumber { get; set; }
        public int AddressId { get; set; }
        [Required(ErrorMessage = "Country is required")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s\-]+$",
                ErrorMessage = "Country must contain only letters")]
        public string Country { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s\-]+$",
             ErrorMessage = "City must contain only letters")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Street is required")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF0-9\s\-\,\.]+$",
             ErrorMessage = "Street can contain letters, numbers, and basic punctuation")]
        public string Street { get; set; } = null!;
        public string? ZipCode { get; set; }
    }
}
