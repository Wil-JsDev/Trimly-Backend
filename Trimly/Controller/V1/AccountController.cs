using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Trimly.Core.Application.DTOs.Account;
using Trimly.Core.Application.DTOs.Account.Authenticate;
using Trimly.Core.Application.DTOs.Account.Password;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Utils;
using RegisterRequest = Trimly.Core.Application.DTOs.Account.Register.RegisterRequest;
using ResetPasswordRequest = Trimly.Core.Application.DTOs.Account.Password.ResetPasswordRequest;

namespace Trimly.Presentation.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/account")]
public class AccountController(
    IAccountService accountService, 
    IValidator<RegisterRequest> validatorRequest,
    IValidator<AuthenticateRequest> validatorAuthenticate,
    IValidator<ForgotRequest> validatorForgotPassword,
    IValidator<ResetPasswordRequest> validatorResetPassword,
    IValidator<UpdateAccountDto> validatorUpdate) : ControllerBase
{
    [HttpPost("owners")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> CreateAccountOwnerAsync([FromBody] RegisterRequest request )
    {
        var resultValidation = await validatorRequest.ValidateAsync(request,new CancellationToken());
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation);
        
        var result = await accountService.RegisterOwnerAsync(request);
        if(result.Success)
            return Ok(result.Data);
        
        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("barbers")]
    [Authorize(Roles = "Owner")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> CreateAccountBarberAsync([FromBody] RegisterRequest request)
    {
        var resultValidation = await validatorRequest.ValidateAsync(request,new CancellationToken());
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation);
        
        var result = await accountService.RegisterAccountAsync(request,Roles.Barber);
        if (result.Success)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }
    
    [HttpPost("clients")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> CreateAccountClientAsync([FromBody] RegisterRequest request)
    {
        var resultValidation = await validatorRequest.ValidateAsync(request,new CancellationToken());
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation);
        
        var result = await accountService.RegisterAccountAsync(request,Roles.Client);
        if (result.Success)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("confirm-account")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> ConfirmAccountAsync([FromQuery] string userId, [FromQuery] string token)
    {   
        var result = await accountService.ConfirmAccountAsync(userId, token);
        if (result.Success)
            return Ok(result.Data);
        
        return NotFound(result.ErrorMessage);
    }
    
    [HttpPost("authenticate")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateRequest request)
    {
        var resultValidation = await validatorAuthenticate.ValidateAsync(request,new CancellationToken());
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation);
        
        var result = await accountService.AuthenticateAsync(request);
        return result.StatusCode switch
        {
            404 => NotFound(ApiResponse<string>.ErrorResponse($" Email {request.Email} not found")),
            400 => BadRequest(ApiResponse<string>.ErrorResponse($"Account not confirmed for {request.Email}")),
            401 => Unauthorized(ApiResponse<string>.ErrorResponse($"Invalid credentials for {request.Email}")),
            _ => Ok(ApiResponse<AuthenticateResponse>.SuccessResponse(result))
        };
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteAccountAsync([FromRoute] string userId)
    {
         await accountService.RemoveAccountAsync(userId);
         return NoContent();
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotRequest request)
    {
        var resultValidation = await validatorForgotPassword.ValidateAsync(request,new CancellationToken());
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation);
        
        var result = await accountService.GetForgotPasswordAsync(request);
        if (result.Success)
            return Ok(result.Data);
        
        return NotFound(result.ErrorMessage);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
    {
        var resultValidation = await validatorResetPassword.ValidateAsync(request,new CancellationToken());
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation);
        
        var result = await accountService.ResetPasswordAsync(request);
        if (result.Success)
            return Ok(result.Data);
        
        return NotFound(result.ErrorMessage);
    }

    [HttpGet("{userId}/details")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> GetDetailsAsync([FromRoute] string userId)
    {
        var result = await accountService.GetAccountDetailsAsync(userId);
        if (result.Success)
            return Ok(result.Data);
        
        return NotFound(result.ErrorMessage);
    }

    [HttpPost("logout")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> LogoutAsync()
    {
        await accountService.LogOutAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateAccountDto request, [FromRoute] string id)
    {
        var resultValidation = await validatorUpdate.ValidateAsync(request,new CancellationToken());
        if (!resultValidation.IsValid)
            return BadRequest(resultValidation);
        
        var result = await accountService.UpdateAccountDetailsAsync(request, id);
        if (result.Success)
            return Ok(result.Data);

        return NotFound(result.ErrorMessage);
    }
}