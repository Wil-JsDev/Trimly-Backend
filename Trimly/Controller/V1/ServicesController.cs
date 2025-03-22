using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Trimly.Core.Application.DTOs.RegisteredCompanies;
using Trimly.Core.Application.DTOs.Service;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Presentation.Validations.Services;

namespace Trimly.Presentation.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/services")]
public class ServicesController(
    IServicesService service,
    IValidator<CreateServiceDTos> validatorCreate, IValidator<UpdateServiceDTos> validatorUpdate) : ControllerBase
{

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetPagedResultAsync(
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
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpPost]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Barber,Owner")]
    public async Task<IActionResult> CreateAsync([FromForm] CreateServiceDTos serviceDTos,CancellationToken cancellationToken)
    {
        var resultValidation = await validatorCreate.ValidateAsync(serviceDTos, cancellationToken);
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation.Errors);
        
        var result = await service.CreateAsync(serviceDTos, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Barber,Owner")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id, 
        [FromBody] UpdateServiceDTos serviceDTos, 
        CancellationToken cancellationToken)
    {
        var resultValidation = await validatorUpdate.ValidateAsync(serviceDTos, cancellationToken);
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation.Errors);
        
        var result = await service.UpdateAsync(id, serviceDTos, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Barber,Owner")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        if (result.IsSuccess)
            return Ok($"The deleted ID is this one {result.Value}");
        
        return NotFound(result.Error);
    }

    [HttpGet("companies/{registeredCompanyId}/services/search/short-duration")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetServiceShortDuration([FromRoute] Guid registeredCompanyId,CancellationToken cancellationToken)
    {
        var result = await service.GetServicesWithDurationLessThan30MinutesAsync(registeredCompanyId,cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("companies/{registeredCompanyId}/services")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetServicesByCompanyId([FromRoute] Guid registeredCompanyId,
        CancellationToken cancellationToken)
    {
        var result = await service.GetServicesByCompanyIdAsync(registeredCompanyId, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("companies/{registeredCompanyId}/services/search/name/{name}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> SearchNameAsync(
        [FromRoute] Guid registeredCompanyId,
        [FromRoute] string name, 
        CancellationToken cancellationToken)
    {
        var result = await service.GetServicesByNameAsync(name, registeredCompanyId, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }
    
    [HttpGet("companies/{registeredCompanyId}/services/search/{price}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> SearchByPriceAsync(
        [FromRoute] Guid registeredCompanyId,
        [FromRoute] decimal price, 
        CancellationToken cancellationToken)
    {
        var result = await service.GetServicesByPriceAsync(price, registeredCompanyId, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("companies/{registeredCompanyId}/services/search/duration-in-minutes/{durationInMinutes}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> SearchByDurationInMinutesAsync(
        [FromRoute] Guid registeredCompanyId,
        int durationInMinutes, 
        CancellationToken cancellationToken)
    {
        var result = await service.GetServicesByDurationMinutesAsync(registeredCompanyId, durationInMinutes, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return NotFound(result.Error);
    }
}