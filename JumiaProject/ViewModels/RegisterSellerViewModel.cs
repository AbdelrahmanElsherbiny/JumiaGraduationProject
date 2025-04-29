using System.ComponentModel.DataAnnotations;


namespace JumiaProject.ViewModels
{
    public class RegisterSellerViewModel
    {
        
            public string Country { get; set; }
            public string Email { get; set; }
            public string VerificationCode { get; set; }
            public DateTime CodeSentTime { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
            public string Phone { get; set; }
            public string ShopType { get; set; } 
            public string ShopName { get; set; }
            public string ShippingAddress { get; set; }
            public bool IsPolicyAccepted { get; set; }
        }

    public class SellerCountryViewModel()
    {
        [Required]
        public string Country { get; set; }
    }
    public class SellerEmailViewModel()
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9._-]+@[a-zA-Z]+.[a-zA-Z]{2,4}[.]{0,1}[a-zA-Z]{0,2}", ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }
    }
    public class SellerCodeViewModel()
    {
        [Required]
        public string Code { get; set; }
    }
    public class SellerPasswordViewModel()
    {
        [Required]
        [MinLength(6)]
        [MaxLength(16)]
        [DataType(DataType.Password)]
        [RegularExpression(@"[A-Za-z_@0-9]{6,15}@*", ErrorMessage = "Invalid Password Format")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password and Confirm Password don't match")]
        public string ConfirmPassword { get; set; }
        [Required]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$",ErrorMessage ="Phone number must be 11 digits")]
        public string Phone { get; set; }
    }
    public class SellerResetNewPasswordViewModel()
    {
        [Required]
        [MinLength(6)]
        [MaxLength(16)]
        [DataType(DataType.Password)]
        [RegularExpression(@"[A-Za-z_@0-9]{6,15}@*", ErrorMessage = "Invalid Password Format")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password don't match")]
        public string ConfirmPassword { get; set; }
    }
    public class SellerShopDetailsViewModel()
    {
        [Required]
        public string ShopType { get; set; }
        [Required]
        public string ShopName { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        [Required]
        public string AboutUs { get; set; }
        [Required]
        public bool IsPolicyAccepted { get; set; }
    }
    public class SellerLoginViewModel()
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

}
