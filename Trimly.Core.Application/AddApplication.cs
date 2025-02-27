using Microsoft.Extensions.DependencyInjection;

namespace Trimly.Core.Application
{
    public static class AddApplication
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            return services;
        }
    }
}
