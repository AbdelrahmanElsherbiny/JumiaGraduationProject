using System.ComponentModel.DataAnnotations;

namespace JumiaProject.ViewModels
{
    public class LoginViewModel
    {
        
        public string Email { get; set; }
       
        public string VerificationCode { get; set; }
      
        public string Password { get; set; }
     
        public DateTime CodeSentTime { get; set; }
    }

    public class UserEmailViewModel()
    {
        [Required]
        [RegularExpression(@"[a-z_0-9]{6,15}@gmail.com", ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }
    }
    public class UserCodeViewModel()
    {
        [Required]
        public string Code { get; set; }
    }
    public class UserPasswordLoginViewModel()
    {
        [Required]
        [MinLength(6)]
        [MaxLength(16)]
        [DataType(DataType.Password)]
        [RegularExpression(@"[A-Za-z_@0-9]{6,15}@*", ErrorMessage = "Invalid Password Format")]
        public string Password { get; set; }
    }
    public class UserPasswordRegisterViewModel()
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
}
