using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Utils;

namespace Trimly.Presentation.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/appointments")]
public class AppointmentController(
    IAppointmentService service,
    IValidator<CreateAppointmentDTos> validatorCreate,
    IValidator<UpdateAppoinmentDTos> validatorUpdate) : ControllerBase
{

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetPagedResulAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPagedResult(pageNumber, pageSize, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpPost]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> CreateAsync([FromBody] CreateAppointmentDTos createAppointmentDTos,CancellationToken cancellationToken)
    {
        var resultValidation = await validatorCreate.ValidateAsync(createAppointmentDTos,cancellationToken);
        if(!resultValidation.IsValid)
            return BadRequest(resultValidation.Errors);
        
        var result = await service.CreateAsync(createAppointmentDTos, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateAppoinmentDTos updateAppointmentDTos,CancellationToken cancellationToken)
    {
        var resultValidation = await validatorUpdate.ValidateAsync(updateAppointmentDTos,cancellationToken);
        if(!resultValidation.IsValid)
            return BadRequest(resultValidation.Errors);
        
        var result = await service.UpdateAsync(id, updateAppointmentDTos, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        if(result.IsSuccess)
            return Ok($"The deleted ID is this one {result.Value}");
        
        return NotFound(result.Error);
    }

    [HttpGet("search/status/{status}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetAppointmentStatusAsync([FromRoute] AppointmentStatus status,
        CancellationToken cancellationToken)
    {
        var result = await service.GetAppointmentsByStatusAsync(status, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpGet("cancel-appointment/{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner,Client")]
    public async Task<IActionResult> CancelAppointmentAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await service.CancelAppointmentAsync(id, cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpPut("reschedule/{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner,Client")]
    public async Task<IActionResult> ReschedulesAppointmentAsync(
        [FromRoute] Guid id,
        [FromBody] RescheduleAppointmentDTos rescheduleAppointmentDTos,
        CancellationToken cancellationToken)
    {
        var result = await service.RescheduleAppointmentAsync(id, rescheduleAppointmentDTos, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }
    
    [HttpPost("confirm-appointment")]
    public IActionResult ConfirmAppointmentInBackground(Guid appointmentId)
    {
        AppointmentQueue.Add(appointmentId);
        return Ok("Quote added for background confirmation");
    }
    
    [HttpPost("{id}/cancel-without-penalty")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner,Client")]
    public async Task<IActionResult> CancelAppointmentWithoutPenaltyAsync(
        [FromRoute] Guid id,
        [FromBody] CancelAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CancelAppointmentWithoutPenaltyAsync(id, request.Reason, cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpPost("{appointmentId}/confirm")]
    [Authorize(Roles = "Client")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> ConfirmAppointmentAsync(
        [FromRoute] Guid appointmentId,
        [FromBody] AppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.ConfirmAppointmentAsync(appointmentId, request.ServiceId, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpPost("{appointmentId}/completed")]
    [Authorize(Roles = "Owner")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> CompleteAppointmentAsync(
        [FromRoute] Guid appointmentId,
        [FromBody] AppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CompletedAppointmentAsync(appointmentId, request.ServiceId, cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }
    
    [HttpGet("count-by-service/{serviceId}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetTotalAppointmentAsync([FromRoute] Guid serviceId, CancellationToken cancellationToken)
    {
        var result = await service.GetTotalAppointmentsCountAsync(serviceId, cancellationToken);
        if (result.IsSuccess)
            return Ok("The number of services are " + result.Value);
        
        return NotFound(result.Error);
    }
    
    
    [HttpGet("{appointmentId}/confirmation-code/{code}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> ConfirmAppointmentCodeAsync(
        [FromRoute] Guid appointmentId,
        [FromRoute] string code,
        CancellationToken cancellationToken)
    {
        var result = await service.ConfirmAppointment(appointmentId, code, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    /* Bug
    [HttpPost("{appointmentId}/cancel-with-penalty")]
    public async Task<IActionResult> CancelWithPenaltyAsync(
        [FromRoute] Guid appointmentId, 
        [FromBody] CancelAppointmentWithPenaltyDto request, 
        CancellationToken cancellationToken)
    {
        var result = await service.CancelAppointmentWithPenaltyAsync(
            appointmentId, 
            request.PenalizationPercentage, 
            cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }
    */
}