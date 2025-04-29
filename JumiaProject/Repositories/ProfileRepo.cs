using System.Net;
using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Repositories
{
    public class ProfileRepo : IProfile
    {
        private readonly JumiaContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;


        public ProfileRepo(JumiaContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public ProfileVM GetUserData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            string userId = id;
            var user = context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null )
            {
                return null;
            }
            var address = context.Addresses.FirstOrDefault(a => a.UserId == userId);

            return new ProfileVM
            {
                User = new UserVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                },
                Address = address == null ? new AddressVM() : new AddressVM

                {
                    City = address?.City,
                    Country = address?.Country,
                    Street = address?.Street
                }
            };

        }
    }
}