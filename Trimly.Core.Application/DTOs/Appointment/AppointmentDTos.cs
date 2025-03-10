using Trimly.Core.Domain.Enum;
namespace Trimly.Core.Application.DTOs.Appointment
{
    public sealed record AppointmentDTos 
    (
        Guid? AppointmentId,
        DateTime? StartDateTime,
        DateTime? EndDateTime,
        AppointmentStatus AppointmentStatus,
        Guid? ServiceId,
        DateTime? CreatedAt,
        DateTime? UpdateAt
    );
}
