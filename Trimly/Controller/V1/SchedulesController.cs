using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Enum;

namespace Trimly.Presentation.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/schedules")]
public class SchedulesController(
    ISchedulesService schedulesService,
    IValidator<CreateSchedulesDTos> validatorCreate,
    IValidator<UpdateSchedulesDTos> validatorUpdate) : ControllerBase
{

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetPagedResult(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await schedulesService.GetPagedResult(pageNumber, pageSize, cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await schedulesService.GetByIdAsync(id, cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpPost]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateSchedulesDTos createSchedulesDTos, CancellationToken cancellationToken)
    {
        var resultValidation = await validatorCreate.ValidateAsync(createSchedulesDTos, cancellationToken);
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation.Errors);
        
        var result = await schedulesService.CreateAsync(createSchedulesDTos, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id, 
        [FromBody] UpdateSchedulesDTos updateSchedulesDTos, 
        CancellationToken cancellationToken)
    {
        var resultValidation = await validatorUpdate.ValidateAsync(updateSchedulesDTos, cancellationToken);
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation.Errors);
        
        var result = await schedulesService.UpdateAsync(id, updateSchedulesDTos, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await schedulesService.DeleteAsync(id, cancellationToken);
        if(result.IsSuccess)
            return Ok($"You deleted this id {result.Value}");
        
        return NotFound(result.Error);
    }

    [HttpPut("company/{registeredCompanyId}/holiday")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> ActivatedHolidayAsync([FromRoute] Guid registeredCompanyId,
        CancellationToken cancellationToken)
    {
        var result = await schedulesService.ActivatedIsHolidayAsync(registeredCompanyId, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpGet("company/{registeredCompanyId}/search/opening-time/{openingTime}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetSchedulesByOpeningTimeAsync(
        [FromRoute] Guid registeredCompanyId,
        [FromRoute] TimeOnly openingTime,
        CancellationToken cancellationToken)
    {
        var result = await schedulesService.FilterByOpeningTimeAsync(registeredCompanyId, openingTime, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpGet("company/{registeredCompanyId}/search/holiday-status/{isHoliday}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetSchedulesByHolidayStatus(
        [FromRoute] Guid registeredCompanyId,
        [FromRoute] Status isHoliday,
        CancellationToken cancellationToken)
    {
        var result = await schedulesService.FilterByIsHolidayAsync(registeredCompanyId, isHoliday, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return NotFound(result.Error);
    }

    [HttpGet("company/{registeredCompanyId}/search/weekday/{weekday}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetSchedulesWeekdayAsync(
        [FromRoute] Guid registeredCompanyId,
        [FromRoute] Weekday weekday,
        CancellationToken cancellationToken)
    {
        var result = await schedulesService.FilterByWeekDayAsync(registeredCompanyId, weekday, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return NotFound(result.Error);
    }

    [HttpGet("company/{registeredCompanyId}/schedules")]
    public async Task<IActionResult> GetSchedulesByCompany(
        [FromRoute] Guid registeredCompanyId,
        CancellationToken cancellationToken)
    {
        var result = await schedulesService.GetSchedulesByCompanyIdAsync(registeredCompanyId, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }
    
}