using Microsoft.AspNetCore.Identity;
using Trimly.Core.Domain.Enum;
using Trimly.Infrastructure.Identity.Models;

namespace Trimly.Infrastructure.Identity.Seeds
{
    public static class DefaultOwnerRoles
    {
        public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            User ownerUser = new()
            {
                UserName = "OwnerDW",
                FirstName = "DW",
                LastName = "DW",
                Email = "trimlydw@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (userManager.Users.All(b => b.Id != ownerUser.Id))
            {
                var user = await userManager.FindByEmailAsync(ownerUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(ownerUser, "SecureP@ss123");
                    await userManager.AddToRoleAsync(ownerUser, Roles.Owner.ToString());
                    await userManager.AddToRoleAsync(ownerUser, Roles.Barber.ToString());
                    await userManager.AddToRoleAsync(ownerUser, Roles.Client.ToString());
                }
            }
        }
    }
}
