using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Application.DTOs.Schedules
{
    public sealed record SchedulesDTos
    (
        Guid? SchedulesId,
        Weekday? Days,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        string? Notes,
        Status? IsHoliday,
        Guid? RegisteredCompanyId,
        DateTime? CreatedAt,
        DateTime? UpdateAt
    );
}
