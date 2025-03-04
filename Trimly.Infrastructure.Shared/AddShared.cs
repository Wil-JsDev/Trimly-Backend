using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Settings;
using Trimly.Infrastructure.Shared.Service;

namespace Trimly.Infrastructure.Shared
{
    public static class AddShared
    {
        public static void AddSharedLayer(this IServiceCollection services, IConfiguration configuration)
        {
            #region Configuration 
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            #endregion

            #region Services
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ICloudinaryService,CloudinaryService>();
            #endregion
        }
    }
}
