using Trimly.Core.Domain.Enum;
namespace Trimly.Core.Application.DTOs.Schedules
{
    public sealed record CreateSchedulesDTos
    (
        Weekday Days,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        string? Notes,
        Guid? RegisteredCompanyId
    );
}
