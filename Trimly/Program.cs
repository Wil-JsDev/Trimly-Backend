using Serilog;
using Trimly.Infrastructure.Persistence;
using Trimly.Infrastructure.Shared;
using Trimly.Infrastructure.Identity;
using Trimly.Infrastructure.Identity.Seeds;
using Trimly.Presentation.BackgroundService;

try
{
    Log.Information("starting server");
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHostedService<ConfirmAppointmentBackgroundService>();
    var config = builder.Configuration;
    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });

    builder.Services.AddPersistenceMethod(config);
    builder.Services.AddSharedLayer(config);
    builder.Services.AddIdentityLayer(config);

    var app = builder.Build();
    await app.Services.SeedDatabaseAsync();
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

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
