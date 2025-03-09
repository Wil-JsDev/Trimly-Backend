using System.Collections;
using Microsoft.Extensions.Logging;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Application.Pagination;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Services;
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repository;
    private readonly ILogger<AppointmentService> _logger;
    public AppointmentService(IAppointmentRepository repository, ILogger<AppointmentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<ResultT<PagedResult<AppointmentDTos>>> GetPagedResult(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0 && pageSize <= 0)
        {
            _logger.LogError("Invalid pagination parameters: pageNumber and pageSize must be greater than 1.");
            
            return ResultT<PagedResult<AppointmentDTos>>.Failure(Error.Failure("400", "Invalid pagination parameters"));
        }

        var appointmentPaged = await _repository.GetPagedResultAsync(pageNumber, pageSize, cancellationToken);
        var dtoItems = appointmentPaged.Items.Select(x => new AppointmentDTos
        (
            AppointmentId: x.AppointmentId,
            StartDateTime: x.StartDateTime,
            EndDateTime: x.EndDateTime,
            AppointmentStatus: x.AppointmentStatus,
            ServiceId: x.ServiceId,
            CreatedAt: x.CreatedAt,
            UpdateAt: x.UpdateAt
        )).ToList();

        if (!dtoItems.Any())
        {
            _logger.LogWarning("No appointments found for the specified parameters. Page: {PageNumber}, Page Size: {PageSize}", pageNumber, pageSize);
    
            return ResultT<PagedResult<AppointmentDTos>>.Failure(Error.Failure("400", "No available appointments found for the specified criteria."));
        }

        PagedResult<AppointmentDTos> pagedResult = new()
        {
            TotalItems = appointmentPaged.TotalItems,
            CurrentPage = appointmentPaged.CurrentPage,
            TotalPages = appointmentPaged.TotalPages,
            Items = dtoItems,
        };
        
        _logger.LogInformation("Returning paged result with {TotalItems} appointments. Page {CurrentPage} of {TotalPages}.", 
            appointmentPaged.TotalItems, appointmentPaged.CurrentPage, appointmentPaged.TotalPages);
        
        return ResultT<PagedResult<AppointmentDTos>>.Success(pagedResult);
    }

    public async Task<ResultT<AppointmentDTos>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
        {
            _logger.LogWarning("No appointment found for the specified {id}.",id);
            
            return ResultT<AppointmentDTos>.Failure(Error.NotFound("404", "No appointment found for the specified."));
        }

        AppointmentDTos appointmentDTos = new AppointmentDTos
        (
            AppointmentId: appointment.AppointmentId,
            StartDateTime: appointment.StartDateTime,
            EndDateTime: appointment.EndDateTime,
            AppointmentStatus: appointment.AppointmentStatus,
            ServiceId: appointment.ServiceId,
            CreatedAt: appointment.CreatedAt,
            UpdateAt: appointment.UpdateAt
        );
        
        _logger.LogInformation("Returning appointment details. AppointmentId: {AppointmentId}, StartDateTime: {StartDateTime}, EndDateTime: {EndDateTime}, Status: {AppointmentStatus}", 
            appointment.AppointmentId, appointment.StartDateTime, appointment.EndDateTime, appointment.AppointmentStatus);
        
        return ResultT<AppointmentDTos>.Success(appointmentDTos);
    }

    public async Task<ResultT<AppointmentDTos>> CreateAsync(CreateAppointmentDTos entityCreateDTo, CancellationToken cancellationToken)
    {
        if (entityCreateDTo == null)
        {
            _logger.LogWarning("No appointment found matching the specified criteria. The request may contain invalid or non-existent appointment details.");
    
            return ResultT<AppointmentDTos>.Failure(Error.Failure("400", "No appointment matches the specified criteria. Please verify your request parameters."));
        }

        var exist = await _repository.ValidateAppointmentAsync(entityCreateDTo.StarDateTime, entityCreateDTo.EndDateTime,cancellationToken);
        if (exist)
        {
            _logger.LogError("Failed to create appointment: The selected time slot ({0} - {1}) is already booked.", entityCreateDTo.StarDateTime, entityCreateDTo.EndDateTime);

            return ResultT<AppointmentDTos>.Failure(Error.Failure("400", 
                $"The selected time slot ({entityCreateDTo.StarDateTime} - {entityCreateDTo.EndDateTime}) is already booked. Please choose a different time."));
        }
        
        Domain.Models.Appointments appointments = new()
        {
            AppointmentId = Guid.NewGuid(),
            StartDateTime = entityCreateDTo.StarDateTime,
            EndDateTime = entityCreateDTo.EndDateTime,
            ServiceId = entityCreateDTo.ServiceId
        };
        
        await _repository.AddAsync(appointments, cancellationToken);

        AppointmentDTos appointmentDTos = new
        (
            AppointmentId: appointments.AppointmentId,
            StartDateTime: appointments.StartDateTime,
            EndDateTime: appointments.EndDateTime,
            AppointmentStatus: AppointmentStatus.Pending,
            ServiceId: appointments.ServiceId,
            CreatedAt: appointments.CreatedAt,
            UpdateAt : appointments.UpdateAt
        );
        
        _logger.LogInformation("Successfully retrieved appointment details. AppointmentId: {AppointmentId}, StartDateTime: {StartDateTime}, EndDateTime: {EndDateTime}, Status: {AppointmentStatus}", 
            appointmentDTos.AppointmentId, appointmentDTos.StartDateTime, appointmentDTos.EndDateTime, appointmentDTos.AppointmentStatus);
        
        return ResultT<AppointmentDTos>.Success(appointmentDTos);
    }

    public async Task<ResultT<AppointmentDTos>> UpdateAsync(Guid id, UpdateAppoinmentDTos entity, CancellationToken cancellation)
    {
        var appointment = await _repository.GetByIdAsync(id, cancellation);
        if (appointment == null)
        {
            _logger.LogError("Appointment not found. AppointmentId: {AppointmentId}", appointment);
    
            return ResultT<AppointmentDTos>.Failure(Error.NotFound("404", "No appointment found for the specified ID."));
        }

        appointment.StartDateTime = entity.StarDateTime;
        appointment.EndDateTime = entity.EndDateTime;
        appointment.ServiceId = entity.ServiceId;
        appointment.UpdateAt = DateTime.UtcNow;
        
        await _repository.UpdateAsync(appointment, cancellation);

        AppointmentDTos apointmentDTos = new
        (
            AppointmentId: appointment.AppointmentId,
            StartDateTime: appointment.StartDateTime,
            EndDateTime: appointment.EndDateTime,
            AppointmentStatus: appointment.AppointmentStatus,
            ServiceId: appointment.ServiceId,
            CreatedAt: appointment.CreatedAt,
            UpdateAt : appointment.UpdateAt
        );
        
        _logger.LogInformation("Successfully retrieved appointment details for AppointmentId: {AppointmentId}. StartDateTime: {StartDateTime}, EndDateTime: {EndDateTime}, Status: {AppointmentStatus}.",
            appointment.AppointmentId, appointment.StartDateTime, appointment.EndDateTime, appointment.AppointmentStatus);

        return ResultT<AppointmentDTos>.Success(apointmentDTos);
    }

    public async Task<ResultT<Guid>> DeleteAsync(Guid id, CancellationToken cancellation)
    {
        var appointment = await _repository.GetByIdAsync(id, cancellation);
        if (appointment == null)
        {
            _logger.LogError("No appointment found with AppointmentId: {AppointmentId}.", id);
    
            return ResultT<Guid>.Failure(Error.NotFound("404", "Appointment not found."));
        }
        
        await _repository.DeleteAsync(appointment, cancellation);
        
        _logger.LogInformation("Successfully deleted appointment with AppointmentId: {AppointmentId}.", id);
        
        return ResultT<Guid>.Success(appointment.AppointmentId ?? Guid.Empty);
    }

    public async Task<ResultT<IEnumerable<AppointmentDTos>>> GetAppointmentsByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken)
    {
        // Validate if the appointment status exists in the database
        var exists = await _repository.ValidateAsync(x => x.AppointmentStatus == status);
        if (!exists)
        {
            _logger.LogError("Appointment status '{Status}' is not valid or does not exist.", status);
        
            return ResultT<IEnumerable<AppointmentDTos>>.Failure(Error.NotFound("404", $"Appointment status '{status}' is not valid or does not exist."));
        }
        
        var filterByStatus = await _repository.FilterByStatusAsync(status, cancellationToken);
        if (filterByStatus == null || !filterByStatus.Any())
        {
            _logger.LogError("No appointments found with status '{Status}'.", status);
        
            return ResultT<IEnumerable<AppointmentDTos>>.Failure(Error.NotFound("404", $"No appointments found with status '{status}'."));
        }
        
        IEnumerable<AppointmentDTos> appointmentDTos = filterByStatus.Select(x => new AppointmentDTos
        (
            AppointmentId: x.AppointmentId,
            StartDateTime: x.StartDateTime,
            EndDateTime: x.EndDateTime,
            AppointmentStatus: x.AppointmentStatus,
            ServiceId: x.ServiceId,
            CreatedAt: x.CreatedAt,
            UpdateAt: x.UpdateAt
        ));

        _logger.LogInformation("{Count} appointments found with status '{Status}'.", appointmentDTos.Count(), status);
    
        return ResultT<IEnumerable<AppointmentDTos>>.Success(appointmentDTos);
    }
    
    public async Task<ResultT<Guid>> CancelAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment == null)
        {
            _logger.LogError("No appointment found with AppointmentId: {AppointmentId}.", appointmentId);
            
            return ResultT<Guid>.Failure(Error.NotFound("404", "Appointment not found.")); 
        }
        
        await _repository.CancelAppointmentAsync(appointment, cancellationToken);
        
        _logger.LogInformation("Successfully canceled appointment with AppointmentId: {AppointmentId}.", appointmentId);
        
        return ResultT<Guid>.Success(appointment.AppointmentId ?? Guid.Empty);
    }

    public async Task<ResultT<RescheduleAppointmentDTos>> RescheduleAppointmentAsync(Guid appointmentId, RescheduleAppointmentDTos rescheduleAppointment,
        CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment == null)
        {
            _logger.LogError("No appointment found with AppointmentId: {AppointmentId}.", appointmentId);
            
            return ResultT<RescheduleAppointmentDTos>.Failure(Error.NotFound("404", "Appointment not found.")); 
        }

        appointment.UpdateAt = DateTime.UtcNow;
        
        await _repository.RescheduleAppointmentAsync(appointment,cancellationToken);
        
        _logger.LogInformation("Successfully rescheduled appointment with AppointmentId: {AppointmentId}.", appointmentId);
        
        return ResultT<RescheduleAppointmentDTos>.Success(rescheduleAppointment);
    }

    public async Task<ResultT<Guid>> ConfirmAppointmentAutomaticallyAsync(Guid appointmentId, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment == null)
        {
            _logger.LogError("No appointment found with AppointmentId: {AppointmentId}.", appointmentId);
        
            return ResultT<Guid>.Failure(Error.NotFound("404", "Appointment not found."));
        }
        
        await _repository.ConfirmAppointmentAutomaticallyAsync(appointment, cancellationToken);
        
        _logger.LogInformation("Appointment with AppointmentId: {AppointmentId} has been successfully confirmed automatically.", appointmentId);
        
        return ResultT<Guid>.Success(appointment.AppointmentId ?? Guid.Empty);
    }

    public async Task<ResultT<Guid>> CancelAppointmentWithoutPenaltyAsync(Guid appointmentId, string cancellationReason,
        CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment == null)
        {
            _logger.LogError("Appointment not found for AppointmentId: {AppointmentId}.", appointmentId);
    
            return ResultT<Guid>.Failure(Error.NotFound("404", $"Appointment with ID {appointmentId} not found."));
        }
        
        appointment.CancellationReason = cancellationReason;
        appointment.UpdateAt = DateTime.UtcNow;
        
        await _repository.CancelAppointmentAsync(appointment, cancellationToken);
        
        _logger.LogInformation("Successfully canceled appointment with AppointmentId: {AppointmentId}.", appointmentId);
        
        return ResultT<Guid>.Success(appointment.AppointmentId ?? Guid.Empty);
    }
    
    public async Task<ResultT<int>> GetTotalAppointmentsCountAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(serviceId, cancellationToken);
        if (appointment == null)
        {
            _logger.LogError("Service with ServiceId: {ServiceId} not found.", serviceId);
            
            return ResultT<int>.Failure(Error.NotFound("404", "Service not found."));
        }
        
        var servicesCount = await _repository.GetTotalAppointmentCountAsync(serviceId, cancellationToken);
        
        _logger.LogInformation("Successfully retrieved total appointment count for ServiceId: {ServiceId}. Total appointments: {TotalAppointments}.", 
            serviceId, servicesCount);
        
        return ResultT<int>.Success(servicesCount); 
    }

    public async Task<ResultT<string>> CancelAppointmentWithPenaltyAsync(Guid appointmentId,
        double penalizationPorcentage, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment == null)
        {
            _logger.LogError("Appointment not found with AppointmentId: {AppointmentId}.", appointmentId);
            
            return ResultT<string>.Failure(Error.NotFound("404", "Appointment not found."));
        }
        if (penalizationPorcentage < 0)
        {
            _logger.LogError("Penalization porcentage cannot be less than zero.");

            return ResultT<string>.Failure(Error.Failure("400", "Penalization percentage cannot be less than zero."));
        }
        
        await _repository.CancelAppointmentWithPenaltyAsync(appointment, penalizationPorcentage, cancellationToken);
        
        _logger.LogInformation("Appointment with AppointmentId: {AppointmentId} has been cancelled with a penalty of {PenalizationPercentage}%.", appointmentId, penalizationPorcentage);

        return ResultT<string>.Success("Appointment successfully cancelled with penalty.");
    }
    
    public async Task<ResultT<ConfirmAppointmentDTos>> ConfirmAppointment(Guid appointmentId, string confirmationCode, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment == null)
        {
            _logger.LogError("Failed to confirm appointment. No appointment found with ID: {AppointmentId}.", appointmentId);
        
            return ResultT<ConfirmAppointmentDTos>.Failure(Error.NotFound("404", "The specified appointment does not exist."));
        }

        if (string.IsNullOrWhiteSpace(confirmationCode))
        {
            _logger.LogError("Failed to confirm appointment. The confirmation code is missing or invalid.");
        
            return ResultT<ConfirmAppointmentDTos>.Failure(Error.Failure("400", "A valid confirmation code is required."));
        }

        appointment.ConfirmationCode = confirmationCode;
        appointment.AppointmentStatus = AppointmentStatus.Confirmed;
        await _repository.UpdateAsync(appointment, cancellationToken);

        ConfirmAppointmentDTos confirmAppointmentDTos = new
        (
            AppointmentStatus: appointment.AppointmentStatus,
            StartDateTime: appointment.StartDateTime,
            EndDateTime: appointment.EndDateTime
        );

        _logger.LogInformation("Appointment with ID: {AppointmentId} successfully confirmed.", appointmentId);
    
        return ResultT<ConfirmAppointmentDTos>.Success(confirmAppointmentDTos);
    }
}