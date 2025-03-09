using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Application.DTOs.Appointment;

public sealed record ConfirmAppointmentDTos
(
    AppointmentStatus AppointmentStatus,
    DateTime? StartDateTime,
    DateTime? EndDateTime
);