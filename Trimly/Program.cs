using Microsoft.AspNetCore.Identity;
using Serilog;
using Trimly.Core.Application;
using Trimly.Infrastructure.Persistence;
using Trimly.Infrastructure.Shared;
using Trimly.Infrastructure.Identity;
using Trimly.Infrastructure.Identity.Models;
using Trimly.Infrastructure.Identity.Seeds;
using Trimly.Presentation.BackgroundService;
using Trimly.Presentation.ServiceExtension;
using Trimly.Presentation.Extensions;

try
{
    Log.Information("starting server");
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddHostedService<ConfirmAppointmentBackgroundService>();

    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var config = builder.Configuration;

    builder.Services.AddPersistenceMethod(config);
    builder.Services.AddSharedLayer(config);
    builder.Services.AddIdentityLayer(config);
    builder.Services.AddApplicationLayer();
    
    //Extensions
    builder.Services.AddVersioning();
    builder.Services.AddSwaggerGen();
    builder.Services.AddException();
    builder.Services.AddValidation();
    builder.Services.AddRateLimiting();
    
    var app = builder.Build();
    
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        try
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            
            await DefaultOwnerRoles.SeedAsync(userManager, roleManager);
            await DefaultRoles.SeedAsync(userManager, roleManager);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during role seeding.");
        }
    }
    
    app.UseExceptionHandler(_ => { });
    //await app.Services.SeedDatabaseAsync();
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseRateLimiter();
    
    app.UseAuthentication();
    
    app.UseAuthorization();

    app.UseSwaggerExtension();
    
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
