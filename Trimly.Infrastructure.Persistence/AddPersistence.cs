using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Infrastructure.Persistence.Context;
using Trimly.Infrastructure.Persistence.Repository;

namespace Trimly.Infrastructure.Persistence
{
    public static class AddPersistence
    {
        public static void AddPersistenceMethod(this IServiceCollection services, IConfiguration configuration)
        {
            #region Connection

            services.AddDbContext<TrimlyContext>(b =>
            {
                 b.UseNpgsql(configuration.GetConnectionString("TrimlyConnection"),
                    c => c.MigrationsAssembly(typeof(TrimlyContext).Assembly.FullName));
            });

            #endregion

            #region  DI
            services.AddTransient<IAppointmentRepository,AppointmentRepository> ();
            services.AddTransient<IRegisteredCompanyRepository, RegisteredCompanyRepository>();
            services.AddTransient<IReviewsRepository, ReviewRepository>();
            services.AddTransient<ISchedulesRepository, SchedulesRepository>();
            services.AddTransient<IServiceRepository, ServiceRepository>();
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            #endregion
        }

    }
}
