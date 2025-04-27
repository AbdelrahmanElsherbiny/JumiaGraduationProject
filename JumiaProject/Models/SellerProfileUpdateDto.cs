namespace JumiaProject.Models
{
    public class SellerProfileUpdateDto
    {
        public string SellerId { get; set; }
        public string StoreName { get; set; }
        public int TaxNumber { get; set; }
        public int? BankAccount { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}