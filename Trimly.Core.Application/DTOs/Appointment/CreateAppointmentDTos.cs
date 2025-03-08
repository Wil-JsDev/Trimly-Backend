
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Application.DTOs.Appointment
{
    public sealed record CreateAppointmentDTos
    (
        DateTime StarDateTime,
        DateTime EndDateTime,
        Guid? ServiceId
    );
}
