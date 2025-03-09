using Trimly.Core.Domain.Enum;
namespace Trimly.Core.Application.DTOs.Schedules
{
    public sealed record UpdateSchedulesDTos
    (
        Weekday Days,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        string? Notes,
        Status? IsHoliday
    ); 
}
