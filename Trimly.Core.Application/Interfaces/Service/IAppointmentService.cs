using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IAppointmentService : IGenericService<
        CreateAppointmentDTos,
        UpdateAppoinmentDTos,
        AppointmentDTos>
    {
        Task<IEnumerable<AppointmentDTos>> GetAppointmentsByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken);

        Task CancelAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken);

        Task RescheduleAppointmentAsync(Guid appointmentId, RescheduleAppointmentDTos rescheduleAppointment, CancellationToken cancellationToken);

        Task ConfirmAppointmentAutomaticallyAsync(AppointmentStatus appointmentStatus, CancellationToken cancellationToken);

        Task CancelAppointmentWithoutPenaltyAsync(Guid appointmentId, string cancellationReason, CancellationToken cancellationToken);

        Task<IEnumerable<AppointmentDTos>> GetAvailableAppointmentsAsync(AppointmentDateFilterType filterType, CancellationToken cancellationToken);

        Task<int> GetTotalAppointmentsCountAsync(Guid serviceId, CancellationToken cancellationToken);

        Task<AppointmentDTos> ConfirmAppointment(Guid appointmentId, string confirmationCode, CancellationToken cancellationToken);
    }
}
