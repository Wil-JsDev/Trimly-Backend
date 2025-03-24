using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Trimly.Core.Application.DTOs.Review;
using Trimly.Core.Application.Interfaces.Service;

namespace Trimly.Presentation.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/review")]
public class ReviewController(
    IReviewService serviceReview,
    IValidator<CreateReviewsDTos> validatorCreate,
    IValidator<ReviewsUpdateDTos> validatorUpdate) : ControllerBase
{
    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetPagedResult(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await serviceReview.GetPagedResult(pageNumber, pageSize, cancellationToken);
        if(result.IsSuccess) 
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await serviceReview.GetByIdAsync(id, cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpPost]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateReviewsDTos createReviewsDTos,CancellationToken cancellationToken)
    {
        var resultValidation = await validatorCreate.ValidateAsync(createReviewsDTos, cancellationToken);
        if(!resultValidation.IsValid)
            return BadRequest(resultValidation.Errors);
        
        var result = await serviceReview.CreateAsync(createReviewsDTos, cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] ReviewsUpdateDTos updateReviewsDTos,
        CancellationToken cancellationToken)
    {
        var resultValidation = await validatorUpdate.ValidateAsync(updateReviewsDTos, cancellationToken);
        if(!resultValidation.IsValid)
            return BadRequest(resultValidation.Errors);
        
        var result = await serviceReview.UpdateAsync(id, updateReviewsDTos, cancellationToken);
        if(result.IsSuccess)
            return Ok(result.Value);
        
        return NotFound(result.Error);  
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await serviceReview.DeleteAsync(id, cancellationToken);
        if(result.IsSuccess)
            return Ok($"This was the id that was removed {result.Value}"); 
        
        return NotFound(result.Error);
    }

    [HttpGet("company/{registeredCompanyId}/average-rating")]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public async Task<IActionResult> GetAverageRatingAsync(Guid registeredCompanyId,
        CancellationToken cancellationToken)
    {
        var result = await serviceReview.GetAverageRatingAsync(registeredCompanyId, cancellationToken);
        if(result.IsSuccess)
            return Ok($"The average rating of this company is {result.Value}");
        
        return BadRequest(result.Error);
    }
}