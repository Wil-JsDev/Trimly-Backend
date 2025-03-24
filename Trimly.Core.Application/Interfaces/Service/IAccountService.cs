using Trimly.Core.Application.DTOs.Account;
using Trimly.Core.Application.DTOs.Account.Authenticate;
using Trimly.Core.Application.DTOs.Account.Password;
using Trimly.Core.Application.DTOs.Account.Register;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Utils;


namespace Trimly.Core.Application.Interfaces.Service;

public interface IAccountService
{
    Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request);
    Task<ApiResponse<string>> ConfirmAccountAsync(string userId, string token);
    Task<ApiResponse<RegisterResponse>> RegisterOwnerAsync(RegisterRequest request);
    Task<ApiResponse<RegisterResponse>> RegisterAccountAsync(RegisterRequest request, Roles role);
    Task RemoveAccountAsync(string userId);
    Task<ApiResponse<ForgotResponse>> GetForgotPasswordAsync(ForgotRequest request);
    Task<ApiResponse<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest request);
    Task<ApiResponse<AccountDto>> GetAccountDetailsAsync(string userId);
    Task LogOutAsync();
    Task<ApiResponse<UpdateAccountDto>> UpdateAccountDetailsAsync(UpdateAccountDto status, string id);

}