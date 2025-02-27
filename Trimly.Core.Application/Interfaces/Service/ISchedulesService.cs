using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface ISchedulesService : IGenericService<
        CreateSchedulesDTos,
        UpdateSchedulesDTos,
        SchedulesDTos>
    {
        Task<bool> ActivatedIsHolidayAsync(CancellationToken cancellationToken);

        Task<IEnumerable<SchedulesDTos>> FilterByOpeningTimeAsync(Guid registeredCompany, TimeOnly openingTime,CancellationToken cancellationToken);

        Task<IEnumerable<SchedulesDTos>> FilterByIsHolidayAsync(Guid registeredCompany, CancellationToken cancellationToken);

        Task<IEnumerable<SchedulesDTos>> FilterByWeekDayAsync(Guid registeredCompany, Weekday weekday,CancellationToken cancellationToken);

        Task<IEnumerable<SchedulesDTos>> GetSchedulesByCompanyIdAsync(Guid registeredCompanyId, CancellationToken cancellationToken);
    }
}
