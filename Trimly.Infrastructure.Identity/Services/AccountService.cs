using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Trimly.Core.Application.DTOs.Account;
using Trimly.Core.Application.DTOs.Account.Authenticate;
using Trimly.Core.Application.DTOs.Account.JWT;
using Trimly.Core.Application.DTOs.Account.Password;
using Trimly.Core.Application.DTOs.Account.Register;
using Trimly.Core.Application.DTOs.Email;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Settings;
using Trimly.Core.Domain.Utils;
using Trimly.Infrastructure.Identity.Models;
using Claim = System.Security.Claims.Claim;

namespace Trimly.Infrastructure.Identity.Services;

public class AccountService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    JWTSettings jwtSettings,
    IEmailService emailSender
    )

    : IAccountService
{
    private async Task<JwtSecurityToken> GenerateTokenAsync(User user)
    {
         var userClaims = await userManager.GetClaimsAsync(user);
         var roles = await userManager.GetRolesAsync(user);
     
     List<Claim> rolesClaims = new List<Claim>();
     
     foreach (var role in roles)
     {
         rolesClaims.Add(new Claim("roles", role));
     }

     var claim = new[]
     {
         new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
         new Claim(JwtRegisteredClaimNames.Email, user.Email),
         new Claim("Id", user.Id)
     }
     .Union(userClaims)
     .Union(rolesClaims);
         
         var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
         var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

         var jwtSecurityToken = new JwtSecurityToken
         (
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claim,
            expires: DateTime.Now.AddMinutes(jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials
         );
         return jwtSecurityToken;
    }

    private RefreshToken GenerateRefreshToken()
    {
        return new RefreshToken
        {
            Token = RandomTokenString(),
            Expired = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow
        };
    }

    private string RandomTokenString()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new Byte[40];
        rng.GetBytes(randomBytes);
        return BitConverter.ToString(randomBytes);
    }
    
    private async Task<string> SendVerificationEmailUrlAsync(User user)
    {
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return code;
    }

    private async Task<string> SendForgotPasswordAsync(User user)
    {
        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return code;
    }
    
    public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request)
    {
        AuthenticateResponse response = new();

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            response.StatusCode = 404;
            return response;
        }
        var result = await signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);

        if (!result.Succeeded)
        {
            response.StatusCode = 401;
            return response;    
        }

        if (!user.EmailConfirmed)
        {
            response.StatusCode = 400;
            return response;
        }
        
        JwtSecurityToken jwtSecurityToken = await GenerateTokenAsync(user);
        
        response.UserId = user.Id;
        response.Username = user.UserName;
        response.FirstName = user.FirstName;
        response.LastName = user.LastName;
        response.Email = user.Email;

        var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
        
        response.Roles = rolesList.ToList();
        response.IsVerified = user.EmailConfirmed;
        response.PhoneNumber = user.PhoneNumber;
        response.JwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var refreshToken = GenerateRefreshToken();
        response.RefreshToken = refreshToken.Token;
        
        return response;
    }
    
    public async Task<ApiResponse<RegisterResponse>> RegisterOwnerAsync(RegisterRequest request)
    {
        RegisterResponse response = new();

        var userSameUsername = await userManager.FindByNameAsync(request.UserName);
        if (userSameUsername != null)
        {
            return ApiResponse<RegisterResponse>.ErrorResponse($"this username {userSameUsername} is already taken");
        }
        
        var userWithEmail = await userManager.FindByEmailAsync(request.Email);
        if (userWithEmail != null)
        {
            return ApiResponse<RegisterResponse>.ErrorResponse($"this email {userWithEmail} is already taken");
        }

        User owner = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            CreateAt = DateTime.UtcNow
        };
        
        var result = await userManager.CreateAsync(owner, request.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(owner, Roles.Owner.ToString());
            response.Email = request.Email;
            response.Username = request.UserName;
            response.UserId = owner.Id;
            
            var verification = await SendVerificationEmailUrlAsync(owner);
            await emailSender.SendAsync(new EmailRequestDTos
            {
                To = request.Email,
                Body = $@"
                <div style='font-family: Arial, sans-serif; color: #F9FAFB; line-height: 1.8; max-width: 600px; margin: 0 auto; border: 1px solid #374151; border-radius: 10px; padding: 25px; background-color: #F2937;'>
                    <h1 style='color: #8B5CF6; font-size: 26px; margin-bottom: 20px; text-align: center;'>Confirm Your Account Registration</h1>
                    <p style='font-size: 16px; margin-bottom: 20px; text-align: center;'>
                        Hello <strong style='color: #F97316;'>{request.Email}</strong>, <br>
                        Thank you for registering as an <strong>Owner</strong> on Trimly. To complete your account setup, please use the verification token provided below.
                    </p>
                    <div style='font-size: 16px; background-color: #374151; padding: 20px; border: 1px dashed #F59E0B; border-radius: 8px; margin-bottom: 20px; text-align: center;'>
                        <strong style='color: #F9FAFB;'>Your Verification Token:</strong>
                        <p style='font-size: 18px; color: #8B5CF6; font-weight: bold;'>{verification}</p>
                    </div>
                    <p style='font-size: 14px; margin-bottom: 20px; text-align: center;'>
                        If you didn’t request this email, no further action is required. Please feel free to contact us if you have any concerns.
                    </p>
                    <hr style='border: none; border-top: 1px solid #374151; margin: 30px 0;'>
                    <p style='font-size: 12px; color: #9CA3AF; text-align: center;'>
                        This email is brought to you by <strong style='color: #F59E0B;'>Trimly</strong>. <br>
                        Please do not reply directly to this email as it is not monitored.
                    </p>
                </div>",
                Subject = "Confirm Your Account Registration"

            });
        }
        else
        {
            return ApiResponse<RegisterResponse>.ErrorResponse($"An error occurred trying to register the user");
        }
        return ApiResponse<RegisterResponse>.SuccessResponse(response);
    }
    
    public async Task<ApiResponse<string>> ConfirmAccountAsync(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse<string>.ErrorResponse($"No account registered with this {userId} user id");
        }
        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        
        var result = await userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded ? ApiResponse<string>.SuccessResponse($"Your account has been successfully confirmed!") 
            : ApiResponse<string>.ErrorResponse($"An error occurred trying to confirm your account");
    }
    
    public async Task<ApiResponse<RegisterResponse>> RegisterAccountAsync(RegisterRequest request, Roles roles)
    {
        RegisterResponse response = new();
        var username = await userManager.FindByNameAsync(request.UserName);
        if (username != null)
        {
            return ApiResponse<RegisterResponse>.ErrorResponse($"this user {request.UserName} is already taken");
        }

        var userWithEmail = await userManager.FindByEmailAsync(request.Email);
        if (userWithEmail != null)
        {
            return ApiResponse<RegisterResponse>.ErrorResponse($"this email {request.Email} is already taken");
        }

        User user = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CreateAt = DateTime.UtcNow
        };
        
        var result = await userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            response.Email = request.Email;
            response.Username = request.UserName;
            response.UserId = user.Id;
            
            await userManager.AddToRoleAsync(user, roles.ToString());
            var verification = await SendVerificationEmailUrlAsync(user);
            await emailSender.SendAsync(new EmailRequestDTos
            {
                To = request.Email,
                Body = $@"
                <div style='font-family: Arial, sans-serif; color: #F9FAFB; line-height: 1.8; max-width: 600px; margin: 0 auto; border: 1px solid #374151; border-radius: 10px; padding: 25px; background-color: #1F2937;'>
                    <h1 style='color: #8B5CF6; font-size: 26px; margin-bottom: 20px; text-align: center;'>Confirm Your Account Registration</h1>
                    <p style='font-size: 16px; margin-bottom: 20px; text-align: center;'>
                        Hello <strong style='color: #F97316;'>{request.Email}</strong>, <br>
                        Thank you for registering with us. To complete your account setup, please use the verification token provided below.
                    </p>
                    <div style='font-size: 16px; background-color: #374151; padding: 20px; border: 1px dashed #F59E0B; border-radius: 8px; margin-bottom: 20px; text-align: center;'>
                        <strong style='color: #F9FAFB;'>Your Verification Token:</strong>
                        <p style='font-size: 18px; color: #8B5CF6; font-weight: bold;'>{verification}</p>
                    </div>
                    <p style='font-size: 14px; margin-bottom: 20px; text-align: center;'>
                        If you didn’t request this email, no further action is required. Please feel free to contact us if you have any concerns.
                    </p>
                    <hr style='border: none; border-top: 1px solid #374151; margin: 30px 0;'>
                    <p style='font-size: 12px; color: #9CA3AF; text-align: center;'>
                        This email is brought to you by <strong style='color: #F59E0B;'>Trimly</strong>. <br>
                        Please do not reply directly to this email as it is not monitored.
                    </p>
                </div>",
                Subject = "Confirm Your Account Registration"
            });
        }
        else
        {
            return ApiResponse<RegisterResponse>.ErrorResponse($"An error occurred trying to register the user");
        }
        return ApiResponse<RegisterResponse>.SuccessResponse(response);
    }

    public async Task RemoveAccountAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await userManager.DeleteAsync(user);
        }
    }

    public async Task<ApiResponse<ForgotResponse>> GetForgotPasswordAsync(ForgotRequest request)
    {
        ForgotResponse response = new();

        var account = await userManager.FindByEmailAsync(request.Email);

        if (account == null)
        {
            return ApiResponse<ForgotResponse>.ErrorResponse("No accounts registered with this email");
        }

        var verification = await SendForgotPasswordAsync(account);
        await emailSender.SendAsync(new EmailRequestDTos
        {
            To = request.Email,
            Body = $@"
            <div style='font-family: Arial, sans-serif; color: #F9FAFB; line-height: 1.8; max-width: 600px; margin: 0 auto; border: 1px solid #374151; border-radius: 10px; padding: 25px; background-color: #111827;'>
                <h1 style='color: #8B5CF6; font-size: 26px; margin-bottom: 20px; text-align: center;'>Reset Your Password</h1>
                <p style='font-size: 16px; margin-bottom: 20px; text-align: center;'>
                    Hello <strong style='color: #F97316;'>{request.Email}</strong>, <br>
                    We received a request to reset your password for your Trimly account. To reset your password, please use the verification code below.
                </p>
                <div style='font-size: 16px; background-color: #374151; padding: 20px; border: 1px dashed #F59E0B; border-radius: 8px; margin-bottom: 20px; text-align: center;'>
                    <strong style='color: #F9FAFB;'>Your Verification Code:</strong>
                    <p style='font-size: 18px; color: #8B5CF6; font-weight: bold;'>{verification}</p>
                </div>
                <p style='font-size: 14px; margin-bottom: 20px; text-align: center;'>
                    If you didn’t request this email, no further action is required. Please feel free to contact us if you have any concerns.
                </p>
                <hr style='border: none; border-top: 1px solid #374151; margin: 30px 0;'>
                <p style='font-size: 12px; color: #9CA3AF; text-align: center;'>
                    This email is brought to you by <strong style='color: #F59E0B;'>Trimly</strong>. <br>
                    Please don't worry about your password; we're here to help.
                </p>
            </div>",

            Subject = "Reset Your Password"

        });

        response.Message = "The email has sent. Check your inbox";
        return ApiResponse<ForgotResponse>.SuccessResponse(response);
    }

    public async Task<ApiResponse<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        ResetPasswordResponse response = new();
        var account = await userManager.FindByEmailAsync(request.Email);

        if (account == null)
        {
            return ApiResponse<ResetPasswordResponse>.ErrorResponse("No accounts registered with this email");
        }
        
        request.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        
        var result = await userManager.ResetPasswordAsync(account, request.Token, request.Password);

        if (!result.Succeeded)
        {
            return ApiResponse<ResetPasswordResponse>.ErrorResponse("An Error has occured trying to reset your password");
        }
        response.Message = "Your password has been reset";
        
        return ApiResponse<ResetPasswordResponse>.SuccessResponse(response);
    }

    public async Task<ApiResponse<AccountDto>> GetAccountDetailsAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return ApiResponse<AccountDto>.ErrorResponse("User account not found");
        
        AccountDto account = new()
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            CreatedAt = user.CreateAt
        };
        return ApiResponse<AccountDto>.SuccessResponse(account);
    }

    public async Task<ApiResponse<AccountDto>> UpdateAccountDetailsAsync(UpdateAccountDto status, string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return ApiResponse<AccountDto>.ErrorResponse("Account not found"); 
        }
        
        user.FirstName = status.FirstName;
        user.LastName = status.LastName;
        user.UserName = status.Username;
            
        var updateUser = await userManager.UpdateAsync(user);

        AccountDto accountDto = new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.UserName
        };
        return ApiResponse<AccountDto>.SuccessResponse(accountDto);
    }
    
    
    public async Task LogOutAsync()
    {
        await signInManager.SignOutAsync();
        
    }
}

