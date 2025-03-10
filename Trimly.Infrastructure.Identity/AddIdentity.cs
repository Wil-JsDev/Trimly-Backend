using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using Trimly.Core.Domain.Settings;
using Trimly.Infrastructure.Identity.Context;
using Trimly.Infrastructure.Identity.Models;

namespace Trimly.Infrastructure.Identity
{
    public static class AddIdentity
    {
        public static void AddIdentityLayer(this IServiceCollection services, IConfiguration configuration)
        {
            #region Identity Connection

            services.AddDbContext<IdentityContext>(b =>
            {
                b.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
                    c => c.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));
            });

            #endregion

            #region Identity

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            #endregion

            #region JWT

            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            #endregion
        }
    }
}
