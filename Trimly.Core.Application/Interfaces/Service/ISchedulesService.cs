﻿using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface ISchedulesService : IGenericService<
        CreateSchedulesDTos,
        UpdateSchedulesDTos,
        SchedulesDTos>
    {
        Task<ResultT<bool>> ActivatedIsHolidayAsync(CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<SchedulesDTos>>> FilterByOpeningTimeAsync(Guid registeredCompany, TimeOnly openingTime, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<SchedulesDTos>>> FilterByIsHolidayAsync(Guid registeredCompany, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<SchedulesDTos>>> FilterByWeekDayAsync(Guid registeredCompany, Weekday weekday, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<SchedulesDTos>>> GetSchedulesByCompanyIdAsync(Guid registeredCompanyId, CancellationToken cancellationToken);

    }
}
