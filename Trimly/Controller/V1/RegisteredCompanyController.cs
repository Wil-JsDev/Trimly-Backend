using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Internal;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.DTOs.RegisteredCompanies;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Enum;
using Trimly.Presentation.Validations.RegisteredCompanies;

namespace Trimly.Presentation.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/registeredCompanies")]
public class RegisteredCompanyController(
    IRegisteredCompaniesService registeredCompanyServices, 
    IValidator<CreateRegisteredCompaniesDTos> validatorCreate,
    IValidator<RegisteredCompaniesUpdateDTos> validatorUpdate) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> CreateAsync([FromForm] CreateRegisteredCompaniesDTos createRegisteredCompaniesDTos,CancellationToken cancellationToken )
    {
        var resultValidation = await validatorCreate.ValidateAsync(createRegisteredCompaniesDTos,cancellationToken);
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation.Errors);
        
        var result = await registeredCompanyServices.CreateAsync(createRegisteredCompaniesDTos, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await registeredCompanyServices.GetByIdAsync(id, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return NotFound(result.Error);
    }

    [HttpPut("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id,[FromForm] RegisteredCompaniesUpdateDTos updateRegisteredCompaniesDTos,CancellationToken cancellationToken)
    {
      var resultValidation = await validatorUpdate.ValidateAsync(updateRegisteredCompaniesDTos, cancellationToken);
      if (!resultValidation.IsValid)
        return BadRequest(resultValidation.Errors);
        
      var result = await registeredCompanyServices.UpdateAsync(id, updateRegisteredCompaniesDTos, cancellationToken);
      if (result.IsSuccess)
          return Ok(result.Value);
        
      return NotFound(result.Error);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await registeredCompanyServices.DeleteAsync(id, cancellationToken);
        if (result.IsSuccess)
            return Ok($"The deleted ID is this one {result.Value}");
        
        return NotFound(result.Error);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber,[FromQuery] int pageSize,CancellationToken cancellationToken)
    {
        var result = await registeredCompanyServices.GetPagedResult(pageNumber, pageSize, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("search/name/{name}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> SearchByNameAsync([FromRoute] string name,CancellationToken cancellationToken)
    {
        var result = await registeredCompanyServices.FilterByNameAsync(name, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }
    
    [HttpGet("search/status/{status}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> SearchByStatusAsync([FromQuery] Status status,CancellationToken cancellationToken)
    {
        var result = await registeredCompanyServices.FilterByStatusAsync(status, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);
    }

    [HttpGet("order-by-name")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> OrderByNameAsync(CancellationToken cancellationToken)
    {
        var result = await registeredCompanyServices.OrderByNameAsync(cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }
    
    [HttpGet("recent")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetRecentAsync(CancellationToken cancellationToken)
    {
        var result = await registeredCompanyServices.GetRecentAsync(cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("ordered")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetOrderedAsync([FromQuery] string orderBy,CancellationToken cancellationToken)
    {
        var result = await registeredCompanyServices.OrderByIdAsync(orderBy, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }
}