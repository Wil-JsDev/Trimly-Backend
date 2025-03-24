using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Trimly.Core.Application.DTOs.Account.JWT;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Settings;
using Trimly.Infrastructure.Identity.Context;
using Trimly.Infrastructure.Identity.Models;
using Trimly.Infrastructure.Identity.Services;

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
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                };
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
                    },

                    OnChallenge = c =>
                    {
                        c.HandleResponse();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new JWTResponse{HasError = true, Error = "An unexpected authentication error occurred"});
                        return c.Response.WriteAsync(result);
                    },
                    OnForbidden = c =>
                    {
                        c.Response.StatusCode = 403;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new JWTResponse { HasError = true, Error = "You aren't Authorized to access to this content" });
                        return c.Response.WriteAsync(result);
                    }
                };

            });
            
            #endregion
            
            #region Services
            services.AddScoped<IAccountService, AccountService>();
            #endregion
        }
    }
}
