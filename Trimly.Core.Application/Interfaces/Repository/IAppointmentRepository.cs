using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface IAppointmentRepository : IGenericRepository<Appointments>
    {
        Task CancelAppointmentWithPenaltyAsync(Appointments appointments, double penalizationPorcentage, CancellationToken cancellationToken);

        Task CancelAppointmentAsync(Appointments appointments, CancellationToken cancellationToken);

        Task RescheduleAppointmentAsync(Appointments appointment, CancellationToken cancellationToken);

        Task ConfirmAppointmentAutomaticallyAsync(Appointments appointments,CancellationToken cancellationToken);

        Task<int> GetTotalAppointmentCountAsync(Guid serviceId, CancellationToken cancellationToken);
        
        Task<IEnumerable<Appointments>> FilterByStatusAsync(AppointmentStatus appointmentStatus, CancellationToken cancellationToken);

        Task<bool> ValidateAppointmentAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);

        Task CompletedAppointmentAsync(Appointments appointment, Domain.Models.Services service, CancellationToken cancellationToken);

        Task ConfirmAppointmentAsync(Appointments appointment, Domain.Models.Services service, CancellationToken cancellationToken);
    }
}
