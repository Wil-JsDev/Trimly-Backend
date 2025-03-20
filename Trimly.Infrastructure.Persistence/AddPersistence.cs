using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Infrastructure.Persistence.Context;
using Trimly.Infrastructure.Persistence.Repository;

namespace Trimly.Infrastructure.Persistence
{
    public static class AddPersistence
    {
        public static void AddPersistenceMethod(this IServiceCollection Services, IConfiguration Configuration)
        {
            #region Connection

            Services.AddDbContext<TrimlyContext>(b =>
            {
                 b.UseSqlServer(Configuration.GetConnectionString("TrimlyConnection"),
                    c => c.MigrationsAssembly(typeof(TrimlyContext).Assembly.FullName));
            });

            #endregion

            #region  DI
            Services.AddTransient<IAppointmentRepository,AppointmentRepository> ();
            Services.AddTransient<IRegisteredCompanyRepository, RegisteredCompanyRepository>();
            Services.AddTransient<IReviewsRepository, ReviewRepository>();
            Services.AddTransient<ISchedulesRepository, SchedulesRepository>();
            Services.AddTransient<IServiceRepository, ServiceRepository>();
            Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            #endregion
        }

    }
}
