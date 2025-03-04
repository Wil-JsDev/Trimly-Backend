using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IAppointmentService : IGenericService<
        CreateAppointmentDTos,
        UpdateAppoinmentDTos,
        AppointmentDTos>
    {
        Task<ResultT<IEnumerable<AppointmentDTos>>> GetAppointmentsByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken);

        Task<ResultT<Guid>> CancelAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken);

        Task<ResultT<RescheduleAppointmentDTos>> RescheduleAppointmentAsync(Guid appointmentId, RescheduleAppointmentDTos rescheduleAppointment, CancellationToken cancellationToken);

        Task<ResultT<Guid>> ConfirmAppointmentAutomaticallyAsync(AppointmentStatus appointmentStatus, CancellationToken cancellationToken);

        Task<ResultT<Guid>> CancelAppointmentWithoutPenaltyAsync(Guid appointmentId, string cancellationReason, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<AppointmentDTos>>> GetAvailableAppointmentsAsync(AppointmentDateFilterType filterType, CancellationToken cancellationToken);

        Task<ResultT<int>> GetTotalAppointmentsCountAsync(Guid serviceId, CancellationToken cancellationToken);

        Task<ResultT<AppointmentDTos>> ConfirmAppointment(Guid appointmentId, string confirmationCode, CancellationToken cancellationToken);
    }
}
