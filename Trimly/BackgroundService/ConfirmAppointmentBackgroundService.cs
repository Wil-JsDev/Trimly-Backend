using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Utils;

namespace Trimly.Presentation.BackgroundService;

public class ConfirmAppointmentBackgroundService(
    ILogger<ConfirmAppointmentBackgroundService> logger,
    IServiceScopeFactory serviceScopeFactory)
    : Microsoft.Extensions.Hosting.BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ConfirmAppointmentBackgroundService start.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessAppointmentAsync();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        logger.LogInformation("ConfirmAppointmentBackgroundService stop.");
    }

    private async Task ProcessAppointmentAsync()
    {
       await Task.Run(async () =>
        {
            using var scope = serviceScopeFactory.CreateScope(); // Creamos un nuevo scope para servicios Scoped
            var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>(); // Obtenemos IAppointmentService

            while (AppointmentQueue.Appointment.Count > 0)
            {
                var appointment = AppointmentQueue.Dequeue(); // Extraemos de la cola
                await appointmentService.ConfirmAppointmentAutomaticallyAsync(appointment, CancellationToken.None);
            }
        });
    }
    
    
}