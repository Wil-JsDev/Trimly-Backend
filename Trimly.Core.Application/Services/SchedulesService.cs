using System.Collections;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Application.Pagination;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Services;

public class SchedulesService : ISchedulesService
{
    private readonly ISchedulesRepository _repository;
    private readonly ILogger<SchedulesService> _logger;
    private readonly IRegisteredCompanyRepository _registeredCompaniesRepository;
    public SchedulesService(ISchedulesRepository repository, ILogger<SchedulesService> logger, IRegisteredCompanyRepository registeredCompaniesRepository)
    {
        _repository = repository;
        _logger = logger;
        _registeredCompaniesRepository = registeredCompaniesRepository;
    }
    public async Task<ResultT<PagedResult<SchedulesDTos>>> GetPagedResult(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters: PageNumber ({0}) and PageSize ({1}) must be greater than zero.", pageNumber, pageSize);
            
            return ResultT<PagedResult<SchedulesDTos>>.Failure(Error.Failure("400", "Invalid pagination parameters. Page number and page size must be greater than zero."));
        }
        
        var schedulesNumber = await _repository.GetPagedResultAsync(pageNumber, pageSize, cancellationToken);
        IEnumerable<SchedulesDTos> schedulesDtos = schedulesNumber.Items.Select(x => new SchedulesDTos
        (
            SchedulesId: x.SchedulesId,
            Days: x.Week,
            OpeningTime: x.OpeningTime,
            ClosingTime: x.ClosingTime,
            Notes: x.Notes,
            IsHoliday: x.IsHoliday,
            RegisteredCompanyId: x.RegisteredCompanyId,
            CreatedAt: x.CreatedAt,
            UpdateAt: DateTime.UtcNow
        ));

        if (!schedulesDtos.Any())
        {
            _logger.LogWarning("No schedules found for the requested pagination parameters: PageNumber ({0}), PageSize ({1}).", pageNumber, pageSize);
            
            return ResultT<PagedResult<SchedulesDTos>>.Failure(Error.Failure("404", "No schedules found for the given page number and page size."));
        }

        PagedResult<SchedulesDTos> pagedResult = new()
        {
            Items = schedulesDtos,
            CurrentPage = schedulesNumber.CurrentPage,
            TotalItems = schedulesNumber.TotalItems,
            TotalPages = schedulesNumber.TotalPages
        };
        
        _logger.LogInformation("Successfully retrieved {0} schedule(s) for Page {1} with Page Size {2}.", schedulesDtos.Count(), pageNumber, pageSize);
        
        return ResultT<PagedResult<SchedulesDTos>>.Success(pagedResult);
    }

    public async Task<ResultT<SchedulesDTos>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var schedule = await _repository.GetByIdAsync(id, cancellationToken);
        if (schedule == null)
        {
            _logger.LogWarning("Schedule with ID {0} was not found.", id);

            return ResultT<SchedulesDTos>.Failure(Error.Failure("404", $"Schedule with ID {id} was not found."));
        }

        SchedulesDTos schedulesDTos = new
        (
            SchedulesId: schedule.SchedulesId,
            Days: schedule.Week,
            OpeningTime: schedule.OpeningTime,
            ClosingTime: schedule.ClosingTime,
            Notes: schedule.Notes,
            IsHoliday: schedule.IsHoliday,
            RegisteredCompanyId: schedule.RegisteredCompanyId,
            CreatedAt: schedule.CreatedAt,
            UpdateAt: DateTime.UtcNow
        );
        
        _logger.LogInformation("Successfully retrieved schedule with ID {0}.", id);
        
        return ResultT<SchedulesDTos>.Success(schedulesDTos);
    }

    public async Task<ResultT<SchedulesDTos>> CreateAsync(CreateSchedulesDTos entityCreateDTo, CancellationToken cancellationToken)
    {
        if (entityCreateDTo == null)
        {
            _logger.LogError("Error: Received a null object in CreateAsync.");
            
            return ResultT<SchedulesDTos>.Failure(Error.Failure("400", "CreateReviewsDTos parameters are required."));
        }

        Domain.Models.Schedules schedules = new()
        {
            SchedulesId = Guid.NewGuid(),
            Week = entityCreateDTo.Days,
            OpeningTime = entityCreateDTo.OpeningTime,
            ClosingTime = entityCreateDTo.ClosingTime,
            Notes = entityCreateDTo.Notes,
            IsHoliday = Status.Deactivated,
            RegisteredCompanyId = entityCreateDTo.RegisteredCompanyId
        };
        
        await _repository.AddAsync(schedules, cancellationToken);
        
        _logger.LogInformation("Creating a new schedule for the company with ID: {RegisteredCompanyId}", entityCreateDTo.RegisteredCompanyId);
        
        SchedulesDTos schedulesDTos = new
        (
            SchedulesId: schedules.SchedulesId,
            Days: schedules.Week,
            OpeningTime: schedules.OpeningTime,
            ClosingTime: schedules.ClosingTime,
            Notes: schedules.Notes,
            IsHoliday: schedules.IsHoliday,
            RegisteredCompanyId: schedules.RegisteredCompanyId,
            CreatedAt: schedules.CreatedAt,
            UpdateAt: DateTime.UtcNow
        );
        
        _logger.LogInformation("Schedule successfully created with ID: {SchedulesId}", schedules.SchedulesId);

        return ResultT<SchedulesDTos>.Success(schedulesDTos);
    }

    public async Task<ResultT<SchedulesDTos>> UpdateAsync(Guid id, UpdateSchedulesDTos entity, CancellationToken cancellation)
    {
       var schedule = await _repository.GetByIdAsync(id, cancellation);
       if (schedule == null)
       {
           _logger.LogError("Error: Schedule with ID {ScheduleId} was not found.", id);
           
           return ResultT<SchedulesDTos>.Failure(Error.Failure("404", "Schedule with ID {0} was not found."));
       }
        
       _logger.LogInformation("Updating schedule with ID: {ScheduleId}", id);
       
       schedule.Week = entity.Days;
       schedule.OpeningTime = entity.OpeningTime;
       schedule.ClosingTime = entity.ClosingTime;
       schedule.Notes = entity.Notes;
       schedule.IsHoliday = entity.IsHoliday;
       
       await _repository.UpdateAsync(schedule, cancellation);

       SchedulesDTos schedulesDTos = new
        (
            SchedulesId: schedule.SchedulesId,
            Days: schedule.Week,
            OpeningTime: schedule.OpeningTime,
            ClosingTime: schedule.ClosingTime,
            Notes: schedule.Notes,
            IsHoliday: schedule.IsHoliday,
            RegisteredCompanyId: schedule.RegisteredCompanyId,
            CreatedAt: schedule.CreatedAt,
            UpdateAt: DateTime.UtcNow
        );
       
        _logger.LogInformation("Schedule with ID {ScheduleId} successfully updated.", id);

       return ResultT<SchedulesDTos>.Success(schedulesDTos);
    }

    public async Task<ResultT<Guid>> DeleteAsync(Guid id, CancellationToken cancellation)
    {
        var schedule = await _repository.GetByIdAsync(id, cancellation);
        if (schedule == null)
        {
            _logger.LogError("Error: Schedule with ID {ScheduleId} was not found.", id);

            return ResultT<Guid>.Failure(Error.NotFound("404", "Schedule with ID {0} was not found."));
        }
        
        _logger.LogInformation("Deleting schedule with ID: {ScheduleId}", id);
        
        await _repository.DeleteAsync(schedule, cancellation);
        
        _logger.LogInformation("Schedule with ID {ScheduleId} successfully deleted.", id);

        return ResultT<Guid>.Success(schedule.SchedulesId ?? Guid.Empty);
    }
    public async Task<ResultT<string>> ActivatedIsHolidayAsync(Guid registeredCompany, CancellationToken cancellationToken)
    {
        var schedules = await _repository.GetScheduleByCompanyIdAsync(registeredCompany, cancellationToken);
        if (schedules == null)
        {
            _logger.LogError("Company with ID ø{CompanyId} was not found.", registeredCompany);
            return ResultT<string>.Failure(Error.NotFound("404", $"Company with ID {registeredCompany} was not found."));
        }

        schedules.IsHoliday = Status.Activated;

        await _repository.UpdateAsync(schedules, cancellationToken);
    
        _logger.LogInformation("Holiday status for Company ID {CompanyId} has been successfully activated.", registeredCompany);

        return ResultT<string>.Success("The holiday status has been successfully activated.");
    }
    
    public async Task<ResultT<IEnumerable<SchedulesDTos>>> FilterByOpeningTimeAsync(Guid registeredCompany, TimeOnly openingTime, CancellationToken cancellationToken)
    {
        var registeredCompanies = await _registeredCompaniesRepository.GetByIdAsync(registeredCompany, cancellationToken);
        if (registeredCompanies == null)
        {
            _logger.LogError("Error: Registered company with ID {CompanyId} was not found.", registeredCompany);

            return ResultT<IEnumerable<SchedulesDTos>>.Failure(Error.NotFound("404", $"Registered company with ID {registeredCompany} was not found."));
        }

        var schedules = await _repository.FilterByOpeningTimeAsync(openingTime, cancellationToken);
        if (schedules == null || !schedules.Any())
        {
            _logger.LogWarning("Warning: No schedules found for registered company ID {CompanyId} with opening time {OpeningTime}.", registeredCompany, openingTime);

            return ResultT<IEnumerable<SchedulesDTos>>.Failure(Error.Failure("400", $"No schedules found for registered company ID {registeredCompany} with opening time {openingTime}."));
        }


        IEnumerable<SchedulesDTos> schedulesDTos = schedules.Select(x => new SchedulesDTos
        (
            SchedulesId: x.SchedulesId,
            Days: x.Week,
            OpeningTime: x.OpeningTime,
            ClosingTime: x.ClosingTime,
            Notes: x.Notes,
            IsHoliday: x.IsHoliday,
            RegisteredCompanyId: x.RegisteredCompanyId,
            CreatedAt: x.CreatedAt,
            UpdateAt: DateTime.UtcNow
        ));
        
        _logger.LogInformation("Schedules successfully retrieved for company ID {CompanyId} with opening time {OpeningTime}.", registeredCompany, openingTime);
        
        return ResultT<IEnumerable<SchedulesDTos>>.Success(schedulesDTos);
    }

    public async Task<ResultT<IEnumerable<SchedulesDTos>>> FilterByIsHolidayAsync(Guid registeredCompany, Status isHoliday, CancellationToken cancellationToken)
    {
        var registeredCompanies = await _registeredCompaniesRepository.GetByIdAsync(registeredCompany, cancellationToken);
        if (registeredCompanies == null)
        {
            _logger.LogError("Error: Registered company with ID {CompanyId} was not found.", registeredCompany);
    
            return ResultT<IEnumerable<SchedulesDTos>>.Failure(Error.NotFound("404", $"Registered company with ID {registeredCompany} was not found."));
        }

        var schedulesIsHoliday = await _repository.FilterByIsHolidayAsync(registeredCompany, isHoliday, cancellationToken);
        if (schedulesIsHoliday == null || !schedulesIsHoliday.Any())
        {
            _logger.LogWarning("Warning: No schedules found for company ID {CompanyId} with holiday status {IsHoliday}.", registeredCompany, isHoliday);
    
            return ResultT<IEnumerable<SchedulesDTos>>.Failure(Error.Failure("400", $"No schedules found for company ID {registeredCompany} with holiday status {isHoliday}."));
        }


        IEnumerable<SchedulesDTos> schedulesDTos = schedulesIsHoliday.Select(x => new SchedulesDTos
        (
            SchedulesId: x.SchedulesId,
            Days: x.Week,
            OpeningTime: x.OpeningTime,
            ClosingTime: x.ClosingTime,
            Notes: x.Notes,
            IsHoliday: x.IsHoliday,
            RegisteredCompanyId: x.RegisteredCompanyId,
            CreatedAt: x.CreatedAt,
            UpdateAt: DateTime.UtcNow
        ));
        
        _logger.LogInformation("Schedules successfully retrieved for company ID {CompanyId} with holiday status {IsHoliday}.", registeredCompany, isHoliday);
        
        return ResultT<IEnumerable<SchedulesDTos>>.Success(schedulesDTos);
    }

    public async Task<ResultT<IEnumerable<SchedulesDTos>>> FilterByWeekDayAsync(Guid registeredCompany, Weekday weekday, CancellationToken cancellationToken)
    {
        var registeredCompanies = await _registeredCompaniesRepository.GetByIdAsync(registeredCompany, cancellationToken);
        if (registeredCompanies == null)
        {
            _logger.LogError("Error: Registered company with ID {CompanyId} was not found.", registeredCompany);
    
            return ResultT<IEnumerable<SchedulesDTos>>.Failure(Error.NotFound("404", $"Registered company with ID {registeredCompany} was not found."));
        }

        var existsWeekDay = await _repository.ValidateAsync(x => x.Week == weekday);
        if (!existsWeekDay)
        {
            _logger.LogWarning("Warning: No schedules found for the specified weekday: {Weekday}.", weekday);
    
            return ResultT<IEnumerable<SchedulesDTos>>.Failure(Error.NotFound("404", $"No schedules found for the specified weekday: {weekday}."));
        }
        
        var schedulesByWeekDay = await _repository.FilterByWeekDayAsync(weekday, cancellationToken);
        if (schedulesByWeekDay == null || !schedulesByWeekDay.Any())
        {
            _logger.LogError("No schedules found for company ID {CompanyId} on weekday {Weekday}.", registeredCompany, weekday);
            
            return ResultT<IEnumerable<SchedulesDTos>>.Failure(Error.Failure("400", "Schedule with ID {0} was not found."));
        }

        IEnumerable<SchedulesDTos> schedulesDTos = schedulesByWeekDay.Select(x => new SchedulesDTos
        (
            SchedulesId: x.SchedulesId,
            Days: x.Week,
            OpeningTime: x.OpeningTime,
            ClosingTime: x.ClosingTime,
            Notes: x.Notes,
            IsHoliday: x.IsHoliday,
            RegisteredCompanyId: x.RegisteredCompanyId,
            CreatedAt: x.CreatedAt,
            UpdateAt: DateTime.UtcNow
        ));
        
        _logger.LogInformation("Schedules successfully retrieved for company ID {CompanyId} on weekday {Weekday}.", registeredCompany, weekday);

        return ResultT<IEnumerable<SchedulesDTos>>.Success(schedulesDTos);
    }

    public async Task<ResultT<IEnumerable<SchedulesDTos>>> GetSchedulesByCompanyIdAsync(Guid registeredCompanyId, CancellationToken cancellationToken)
    {
        var registeredCompanies = await _registeredCompaniesRepository.GetByIdAsync(registeredCompanyId, cancellationToken);
        if (registeredCompanies == null)
        {
            _logger.LogError("Error: Registered company with ID {CompanyId} was not found.", registeredCompanyId);
    
            return ResultT<IEnumerable<SchedulesDTos>>.Failure(Error.NotFound("404", $"Registered company with ID {registeredCompanyId} was not found."));
        }

        var schedulesByCompany = await _repository.GetSchedulesByCompanyId(registeredCompanyId, cancellationToken);
        if (schedulesByCompany == null || !schedulesByCompany.Any())
        {
            _logger.LogWarning("Warning: No schedules found for company ID {CompanyId}.", registeredCompanyId);

            return ResultT<IEnumerable<SchedulesDTos>>.Failure(Error.Failure("400", $"No schedules found for company ID {registeredCompanyId}."));
        }
        
        IEnumerable<SchedulesDTos> schedulesDTos = schedulesByCompany.Select(x => new SchedulesDTos
        (
            SchedulesId: x.SchedulesId,
            Days: x.Week,
            OpeningTime: x.OpeningTime,
            ClosingTime: x.ClosingTime,
            Notes: x.Notes,
            IsHoliday: x.IsHoliday,
            RegisteredCompanyId: x.RegisteredCompanyId,
            CreatedAt: x.CreatedAt,
            UpdateAt: DateTime.UtcNow
        ));
        
        _logger.LogInformation("Schedules successfully retrieved for company ID {CompanyId}.", registeredCompanyId);
        
        return ResultT<IEnumerable<SchedulesDTos>>.Success(schedulesDTos);
    }
}