using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            //.AddEntityFrameworkStores<JumiaContext>();


            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<JumiaContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ISeller, SellerRepo>();
            builder.Services.AddScoped<IHome, HomeRepo>();
            builder.Services.AddScoped<ICategory, CategoryRepo>();
            builder.Services.AddScoped<IProduct, ProductRepo>();
            builder.Services.AddScoped<IOrder, OrderRepo>();
            builder.Services.AddScoped<IUser, UserRepo>();


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
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
