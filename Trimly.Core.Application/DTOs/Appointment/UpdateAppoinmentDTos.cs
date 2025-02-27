
namespace Trimly.Core.Application.DTOs.Appointment
{
    public sealed record UpdateAppoinmentDTos
    (
        DateTime? StarDateTime,
        DateTime? EndDateTime,
        string? CancellationReason,
        Guid? ServiceId
    );
}
