using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trimly.Infrastructure.Identity.Models;

namespace Trimly.Infrastructure.Identity.Seeds
{
    public static class SeedDataExtension
    {
        public static async Task SeedDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var logger = scopedServices.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

            try
            {
                var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

                await DefaultOwnerRoles.SeedAsync(userManager, roleManager);
                await DefaultRoles.SeedAsync(userManager, roleManager);

                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while seeding roles and users");
            }
        }
    }
}
