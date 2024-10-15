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

                context.Admins.Add(
                    new Admin { Username = "Admin", HashedPassword = "bL2WlNy4Dm0s0pX88f11WJm0hJ+uZKzIkMFrfs78Mnw=", Role = AdminRole.SuperAdmin }
                );
                context.SaveChanges();
            }
        }
    }
}
