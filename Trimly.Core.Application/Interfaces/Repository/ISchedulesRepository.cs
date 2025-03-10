using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface ISchedulesRepository : IGenericRepository<Schedules>
    {
        Task<IEnumerable<Schedules>> FilterByOpeningTimeAsync(TimeOnly openingTime, CancellationToken cancellationToken);

        Task<IEnumerable<Schedules>> FilterByIsHolidayAsync(Guid registeredCompanyId, Status holidayStatus, CancellationToken cancellationToken);

        Task<IEnumerable<Schedules>> FilterByWeekDayAsync(Weekday weekday, CancellationToken cancellationToken);

        Task<IEnumerable<Schedules>> GetSchedulesByCompanyId(Guid registeredCompanyId, CancellationToken cancellationToken);

        Task<Schedules> GetScheduleByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);
    }
}
