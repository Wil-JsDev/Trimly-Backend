using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Trimly.Core.Application.DTOs.RegisteredCompanies;
using Trimly.Core.Application.DTOs.Service;
using Trimly.Core.Application.Interfaces.Service;

namespace Trimly.Presentation.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/services")]
public class ServicesController(
    IServicesService service,
    IValidator<CreateServiceDTos> validatorCreate,
    IValidator<UpdateServiceDTos> validatorUpdate) : ControllerBase
{

    [HttpGet("pagination")]
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
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpPost]
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
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        if (result.IsSuccess)
            return Ok($"The deleted ID is this one {result.Value}");
        
        return NotFound(result.Error);
    }

    [HttpPost("companies/{registeredCompanyId}/services/{serviceId}/apply-discount")]
    public async Task<IActionResult> ApplyDiscountCodeAsync(
        [FromRoute] Guid registeredCompanyId, 
        [FromRoute] Guid serviceId, 
        [FromQuery] string discountCode,
        CancellationToken cancellationToken)
    {
        var result = await service.ApplyDiscountCodeAsync(
            serviceId, 
            registeredCompanyId, 
            discountCode, 
            cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("companies/{registeredCompanyId}/services/search/short-duration")]
    public async Task<IActionResult> GetServiceShortDuration([FromRoute] Guid registeredCompanyId,CancellationToken cancellationToken)
    {
        var result = await service.GetServicesWithDurationLessThan30MinutesAsync(registeredCompanyId,cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("companies/{registeredCompanyId}/services")]
    public async Task<IActionResult> GetServicesByCompanyId([FromRoute] Guid registeredCompanyId,
        CancellationToken cancellationToken)
    {
        var result = await service.GetServicesByCompanyIdAsync(registeredCompanyId, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("companies/{registeredCompanyId}/services/search/name/{name}")]
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
    
}