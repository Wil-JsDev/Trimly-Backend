using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface IAppointmentRepository : IGenericRepository<Appointments>
    {
        Task CancelAppointmentWithPenaltyAsync(Appointments appointments, CancellationToken cancellationToken);

        Task CancelAppointmentAsync(Appointments appointments, string cancellationReason, CancellationToken cancellationToken);

        Task<IEnumerable<Appointments>> GetAvailableAppointmentsAsync(DateTime dateTime,CancellationToken cancellationToken);

        Task RescheduleAppointmentAsync(Appointments appointment, CancellationToken cancellationToken);

        Task ConfirmAppointmentAutomaticallyAsync(AppointmentStatus appointmentStatus,CancellationToken cancellationToken);

        Task<int> GetTotalAppointmentCountAsync(Guid serviceId, CancellationToken cancellationToken);

        Task<IEnumerable<Appointments>> FilterByStatusAsync(AppointmentStatus appointmentStatus, CancellationToken cancellationToken);
    }
}
