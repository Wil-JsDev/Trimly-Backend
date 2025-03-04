using Microsoft.EntityFrameworkCore;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;
using Trimly.Infrastructure.Persistence.Context;

namespace Trimly.Infrastructure.Persistence.Repository
{
    public class SchedulesRepository: GenericRepository<Schedules>, ISchedulesRepository
    {
        public SchedulesRepository(TrimlyContext context) : base(context){ }

        public async Task<IEnumerable<Schedules>> FilterByIsHolidayAsync(Guid registeredCompany, Status holidayStatus, CancellationToken cancellationToken) =>
            await _context.Set<Schedules>()
            .AsNoTracking()
            .Where(c => c.RegisteredCompanyId == registeredCompany && c.IsHoliday == holidayStatus)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Schedules>> FilterByOpeningTimeAsync(TimeOnly openingTime, CancellationToken cancellationToken) => 
            await _context.Set<Schedules>()
            .AsNoTracking()
            .Where(o => o.OpeningTime == openingTime)
            .Include(s => s.RegisteredCompanies)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Schedules>> FilterByWeekDayAsync(Weekday weekday, CancellationToken cancellationToken) => 
            await _context.Set<Schedules>()
            .AsNoTracking()
            .Where(w => w.Week == weekday)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Schedules>> GetSchedulesByCompanyId(Guid companyId, CancellationToken cancellationToken) => 
            await _context.Set<Schedules>()
            .AsNoTracking()
            .Where(s => s.RegisteredCompanyId == companyId)
            .Include(s => s.RegisteredCompanies)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }
}
