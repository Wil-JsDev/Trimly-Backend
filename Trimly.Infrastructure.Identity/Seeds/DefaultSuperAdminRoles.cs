using Microsoft.AspNetCore.Identity;
using Trimly.Core.Domain.Enum;
using Trimly.Infrastructure.Identity.Models;

namespace Trimly.Infrastructure.Identity.Seeds
{
    public static class DefaultSuperAdminRoles
    {
        public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            User role = new()
            {
                UserName = "SuperAdminWD",
                FirstName = "Super",
                LastName = "Admin",
                Email = "trimlydw@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (userManager.Users.All(b => b.Id != role.Id))
            {
                var user = await userManager.FindByEmailAsync(role.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(role, "SecureP@ss123");
                    await userManager.AddToRoleAsync(role, Roles.SuperAdministrador.ToString());
                }
            }
        }
    }
}
