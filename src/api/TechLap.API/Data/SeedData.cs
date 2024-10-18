using Microsoft.EntityFrameworkCore;
using TechLap.API.Enums;
using TechLap.API.Models;

namespace TechLap.API.Data
{

    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TechLapContext(
                serviceProvider.GetRequiredService<DbContextOptions<TechLapContext>>()))
            {
                if (context.Admins.Any() || context.Users.Any() || context.Orders.Any() || context.OrderDetails.Any() || context.Categories.Any() || context.Discounts.Any() || context.Products.Any())
                {
                    return;
                }

                //Password: Admin@@
                context.Admins.Add(
                    new Admin { Username = "Admin", HashedPassword = "5rN5po7B+Hf0KLUY5IOr+GxkiqqxbEbD8QV3ipEdojY=", Role = AdminRole.SuperAdmin }
                );
                context.SaveChanges();
            }
        }
    }
}
