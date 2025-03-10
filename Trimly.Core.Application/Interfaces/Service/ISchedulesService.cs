using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface ISchedulesService : IGenericService<
        CreateSchedulesDTos,
        UpdateSchedulesDTos,
        SchedulesDTos>
    {
        Task<ResultT<string>> ActivatedIsHolidayAsync(Guid registeredCompany,CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<SchedulesDTos>>> FilterByOpeningTimeAsync(Guid registeredCompany, TimeOnly openingTime, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<SchedulesDTos>>> FilterByIsHolidayAsync(Guid registeredCompany, Status isHoliday ,CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<SchedulesDTos>>> FilterByWeekDayAsync(Guid registeredCompany, Weekday weekday, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<SchedulesDTos>>> GetSchedulesByCompanyIdAsync(Guid registeredCompanyId, CancellationToken cancellationToken);

    }
}
