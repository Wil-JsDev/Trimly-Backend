using Microsoft.Extensions.DependencyInjection;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Application.Services;

namespace Trimly.Core.Application
{
    public static class AddApplication
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddScoped<IRegisteredCompaniesService, RegisteredCompaniesService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ISchedulesService, SchedulesService>();
            return services;
        }
    }
}
