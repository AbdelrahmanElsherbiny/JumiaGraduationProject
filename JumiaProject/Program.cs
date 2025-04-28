using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace JumiaProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<JumiaContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(connectionString));
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddRazorPages();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<JumiaContext>();

            builder.Services.AddAuthorization();

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ISeller, SellerRepo>();
            builder.Services.AddScoped<IHome, HomeRepo>();
            builder.Services.AddScoped<ICategory, CategoryRepo>();
            builder.Services.AddScoped<IProduct, ProductRepo>();
            builder.Services.AddScoped<IOrder, OrderRepo>();
            builder.Services.AddScoped<IUser, UserRepo>();
            builder.Services.AddScoped<IAdmin, AdminRepo>();
            builder.Services.AddScoped<ICategory, CategoryRepo>();
            builder.Services.AddScoped<ISize, SizeRepo>();
            builder.Services.AddScoped<ICategorySize, CategorySizeRepo>();
            builder.Services.AddScoped<ICart, CartRepo>();
            builder.Services.AddScoped<IWishlist, WishlistRepo>();
            builder.Services.AddScoped<IProfile, ProfileRepo>();
            builder.Services.AddScoped<IAddress, AddressRepo>();
            builder.Services.AddScoped<IShippingTracking, ShippingTrackingRepo>();
            builder.Services.AddScoped<IProductVariant, ProductVariantRepo>();
            builder.Services.AddScoped<IOrderItem, OrderItemRepo>();
            builder.Services.AddScoped<IPayment, PaymentRepo>();
            builder.Services.AddScoped<ICartItem, CartItemRepo>();

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ChatGptService>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
