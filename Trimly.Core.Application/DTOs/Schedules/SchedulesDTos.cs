using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Application.DTOs.Schedules
{
    public sealed record SchedulesDTos
    (
        Guid? ShedulesId,
        Weekday? Days,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        string? Notes,
        Status? IsHolady,
        Guid? RegisteredCompanyId,
        DateTime? CreatedAt,
        DateTime? UpdateAt
    );
}
