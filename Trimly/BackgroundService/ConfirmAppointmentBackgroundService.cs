using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Utils;

namespace Trimly.Presentation.BackgroundService;

public class ConfirmAppointmentBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<ConfirmAppointmentBackgroundService> _logger;
    private readonly AppointmentQueue _queue;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ConfirmAppointmentBackgroundService(
        IAppointmentService service, 
        ILogger<ConfirmAppointmentBackgroundService> logger,
        AppointmentQueue queue,
        IServiceScopeFactory serviceScopeFactory)
    {
        _appointmentService = service;
        _logger = logger;
        _queue = queue;
        _serviceScopeFactory = serviceScopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ConfirmAppointmentBackgroundService start.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var appointmentId = await _queue.DequeueAsync(stoppingToken);
            if (appointmentId == null) continue;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();

                _logger.LogInformation("Processing automatic confirmation for appointment: {AppointmentId}", appointmentId);

                await appointmentService.ConfirmAppointmentAutomaticallyAsync(appointmentId.Value, stoppingToken);
            }
        }

        _logger.LogInformation("ConfirmAppointmentBackgroundService stop.");
        
    }
}