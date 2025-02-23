using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trimly.Infrastructure.Persistence.Context;

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
        }

    }
}
