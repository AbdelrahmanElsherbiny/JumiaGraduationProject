using Microsoft.AspNetCore.Identity;

namespace JumiaProject.Models
{
    public class ApplicationUser:IdentityUser
    {
       public  bool IsDeleted { get; set;}
       public virtual Seller Seller { get; set;}

       public virtual List<Review> Reviews { get; set;}=new List<Review>();

        public virtual List<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
       public virtual Cart Cart { get; set; } = null!;
       public virtual List<Order> Orders { get; set; }=new List<Order>();
       public virtual List<Address> Addresses { get; set; } =new List<Address>();
    }
}
