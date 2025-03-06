﻿using Microsoft.AspNetCore.Identity;
using Trimly.Core.Domain.Enum;
using Trimly.Infrastructure.Identity.Models;

namespace Trimly.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdministrador.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Administrador.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Barbero.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Cliente.ToString()));
        }
    }
}
