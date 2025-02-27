using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface ISchedulesRepository : IGenericRepository<Schedules>
    {
        Task<IEnumerable<Schedules>> FilterByOpeningTimeAsync(TimeOnly openingTime, Guid registeredCompany,CancellationToken cancellationToken);

        Task<IEnumerable<Schedules>> FilterByIsHolidayAsync(Guid registeredCompany, CancellationToken cancellationToken);

        Task<IEnumerable<Schedules>> FilterByWeekDayAsync(Weekday weekday, Guid registeredCompany, CancellationToken cancellationToken);

        Task<IEnumerable<Schedules>> GetSchedulesByCompanyId(Guid companyId,CancellationToken cancellationToken);
    }
}
