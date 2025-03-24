using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
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
    IOptions<JWTSettings> jwtSettings,
    IEmailService emailSender
    ) : IAccountService
{
    private JWTSettings _JwtSettings {get;} = jwtSettings.Value;
    
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
            Email = request.Email
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
                <div
  style=""
    font-family: 'Segoe UI', Roboto, Arial, sans-serif;
    color: #f9fafb;
    line-height: 1.6;
    max-width: 600px;
    margin: 0 auto;
    border-radius: 16px;
    padding: 0;
    background-color: #111827;
    overflow: hidden;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
  ""
>
  <!-- Header with gradient -->
  <div
    style=""
      background: linear-gradient(
        135deg,
        #8b5cf6 0%,
        #6d28d9 50%,
        #4c1d95 100%
      );
      padding: 30px 20px;
      text-align: center;
    ""
  >
    <div
      style=""
        display: inline-block;
        background-color: rgba(255, 255, 255, 0.15);
        border-radius: 50px;
        padding: 8px 16px;
        margin-bottom: 20px;
      ""
    >
      <span
        style=""
          color: #ffffff;
          font-weight: 600;
          font-size: 14px;
          letter-spacing: 1px;
        ""
        >TRIMLY ACCOUNT</span
      >
    </div>
    <h1
      style=""
        color: #ffffff;
        font-size: 32px;
        margin: 0 0 10px 0;
        font-weight: 800;
        text-align: center;
        letter-spacing: -0.5px;
      ""
    >
      Verify Your Account
    </h1>
    <p
      style=""
        color: rgba(255, 255, 255, 0.9);
        font-size: 16px;
        margin: 0;
        font-weight: 400;
      ""
    >
      Complete your registration in one simple step
    </p>
  </div>

  <!-- Main content area with subtle gradient -->
  <div
    style=""
      background: linear-gradient(180deg, #1f2937 0%, #111827 100%);
      padding: 40px 30px;
    ""
  >
    <!-- Welcome message card -->
    <div
      style=""
        background-color: rgba(255, 255, 255, 0.05);
        border-left: 4px solid #f59e0b;
        border-radius: 8px;
        padding: 20px;
        margin-bottom: 30px;
      ""
    >
      <p style=""font-size: 17px; margin: 0; line-height: 1.7"">
        Hello
        <strong style=""color: #f97316; font-weight: 600"">{request.Email}</strong
        >,
        <br />
        Thank you for registering as an
        <span
          style=""
            display: inline-block;
            background-color: rgba(139, 92, 246, 0.2);
            border: 1px solid rgba(139, 92, 246, 0.4);
            border-radius: 4px;
            padding: 2px 8px;
            font-size: 14px;
            color: #a78bfa;
            font-weight: 600;
          ""
          >Owner</span
        >
        on Trimly. We're excited to have you on board!
      </p>
    </div>

    <!-- Verification token box -->
    <div style=""text-align: center; margin-bottom: 35px"">
      <p
        style=""
          font-size: 16px;
          color: #9ca3af;
          margin-bottom: 15px;
          font-weight: 500;
        ""
      >
        Please use the verification code below to complete your account setup:
      </p>

      <div
        style=""
          background: linear-gradient(145deg, #2d3748, #1e293b);
          border: 1px solid rgba(139, 92, 246, 0.3);
          border-radius: 12px;
          padding: 25px;
          position: relative;
          box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        ""
      >
        <!-- Decorative elements -->
        <div
          style=""
            position: absolute;
            top: -10px;
            left: 20px;
            background-color: #f59e0b;
            width: 20px;
            height: 20px;
            border-radius: 50%;
            opacity: 0.1;
          ""
        ></div>
        <div
          style=""
            position: absolute;
            bottom: -5px;
            right: 30px;
            background-color: #8b5cf6;
            width: 15px;
            height: 15px;
            border-radius: 50%;
            opacity: 0.1;
          ""
        ></div>

        <p
          style=""
            font-size: 14px;
            color: #a78bfa;
            text-transform: uppercase;
            letter-spacing: 1.5px;
            margin: 0 0 10px 0;
            font-weight: 600;
          ""
        >
          Verification Code
        </p>
        <div
          style=""
            font-size: 12px;
            color: #f59e0b;
            font-weight: 700;
            letter-spacing: 5px;
            margin: 0;
            font-family: 'Courier New', monospace;
          ""
        >
          {verification}
        </div>
        <p style=""font-size: 12px; color: #9ca3af; margin-top: 15px"">
          This code will expire in 30 minutes
        </p>
      </div>
    </div>

    <!-- Action button -->
    <div style=""text-align: center; margin-bottom: 35px"">
      <a
        href=""#""
        style=""
          display: inline-block;
          background: linear-gradient(to right, #f59e0b, #f97316);
          color: #ffffff;
          font-weight: 600;
          padding: 14px 30px;
          border-radius: 8px;
          text-decoration: none;
          font-size: 16px;
          box-shadow: 0 4px 10px rgba(249, 115, 22, 0.3);
        ""
        >Verify My Account</a
      >
    </div>

    <!-- Security note -->
    <div
      style=""
        background-color: rgba(255, 255, 255, 0.03);
        border-radius: 8px;
        padding: 15px;
        margin-bottom: 30px;
        border: 1px solid rgba(255, 255, 255, 0.05);
      ""
    >
      <p style=""font-size: 14px; color: #9ca3af; margin: 0; line-height: 1.6"">
        <span style=""color: #f59e0b; font-weight: 600"">Security Note:</span> If
        you didn't request this email, please ignore it or contact our support
        team immediately. Your account security is important to us.
      </p>
    </div>
  </div>

  <!-- Footer -->
  <div
    style=""
      background-color: rgba(17, 24, 39, 0.8);
      padding: 30px;
      text-align: center;
      border-top: 1px solid rgba(255, 255, 255, 0.1);
    ""
  >
    <div style=""margin-bottom: 20px"">
      <span
        style=""
          display: inline-block;
          background: linear-gradient(to right, #8b5cf6, #f59e0b);
          -webkit-background-clip: text;
          -webkit-text-fill-color: transparent;
          font-size: 24px;
          font-weight: 800;
        ""
        >Trimly</span
      >
    </div>
    <p style=""font-size: 13px; color: #6b7280; margin: 0 0 15px 0"">
      This is an automated message, please do not reply directly to this email.
    </p>
    <div style=""margin-top: 20px"">
      <a
        href=""#""
        style=""
          color: #9ca3af;
          text-decoration: none;
          font-size: 13px;
          margin: 0 10px;
        ""
        >Help Center</a
      >
      <a
        href=""#""
        style=""
          color: #9ca3af;
          text-decoration: none;
          font-size: 13px;
          margin: 0 10px;
        ""
        >Privacy Policy</a
      >
      <a
        href=""#""
        style=""
          color: #9ca3af;
          text-decoration: none;
          font-size: 13px;
          margin: 0 10px;
        ""
        >Terms of Service</a
      >
    </div>
    <p style=""font-size: 12px; color: #4b5563; margin-top: 20px"">
      © 2025 Trimly. All rights reserved.
    </p>
  </div>
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
                <div
  style=""
    font-family: 'Segoe UI', Roboto, Arial, sans-serif;
    color: #f9fafb;
    line-height: 1.6;
    max-width: 600px;
    margin: 0 auto;
    border-radius: 16px;
    padding: 0;
    background-color: #111827;
    overflow: hidden;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
  ""
>
  <!-- Header with gradient -->
  <div
    style=""
      background: linear-gradient(
        135deg,
        #8b5cf6 0%,
        #6d28d9 50%,
        #4c1d95 100%
      );
      padding: 30px 20px;
      text-align: center;
    ""
  >
    <div
      style=""
        display: inline-block;
        background-color: rgba(255, 255, 255, 0.15);
        border-radius: 50px;
        padding: 8px 16px;
        margin-bottom: 20px;
      ""
    >
      <span
        style=""
          color: #ffffff;
          font-weight: 600;
          font-size: 14px;
          letter-spacing: 1px;
        ""
        >TRIMLY ACCOUNT</span
      >
    </div>
    <h1
      style=""
        color: #ffffff;
        font-size: 32px;
        margin: 0 0 10px 0;
        font-weight: 800;
        text-align: center;
        letter-spacing: -0.5px;
      ""
    >
      Verify Your Account
    </h1>
    <p
      style=""
        color: rgba(255, 255, 255, 0.9);
        font-size: 16px;
        margin: 0;
        font-weight: 400;
      ""
    >
      Complete your registration in one simple step
    </p>
  </div>

  <!-- Main content area with subtle gradient -->
  <div
    style=""
      background: linear-gradient(180deg, #1f2937 0%, #111827 100%);
      padding: 40px 30px;
    ""
  >
    <!-- Welcome message card -->
    <div
      style=""
        background-color: rgba(255, 255, 255, 0.05);
        border-left: 4px solid #f59e0b;
        border-radius: 8px;
        padding: 20px;
        margin-bottom: 30px;
      ""
    >
      <p style=""font-size: 17px; margin: 0; line-height: 1.7"">
        Hello
        <strong style=""color: #f97316; font-weight: 600"">{request.Email}</strong
        >,
        <br />
        Thank you for registering as on Trimly.
      </p>
    </div>

    <!-- Verification token box -->
    <div style=""text-align: center; margin-bottom: 35px"">
      <p
        style=""
          font-size: 16px;
          color: #9ca3af;
          margin-bottom: 15px;
          font-weight: 500;
        ""
      >
        Please use the verification code below to complete your account setup:
      </p>

      <div
        style=""
          background: linear-gradient(145deg, #2d3748, #1e293b);
          border: 1px solid rgba(139, 92, 246, 0.3);
          border-radius: 12px;
          padding: 25px;
          position: relative;
          box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        ""
      >
        <!-- Decorative elements -->
        <div
          style=""
            position: absolute;
            top: -10px;
            left: 20px;
            background-color: #f59e0b;
            width: 20px;
            height: 20px;
            border-radius: 50%;
            opacity: 0.1;
          ""
        ></div>
        <div
          style=""
            position: absolute;
            bottom: -5px;
            right: 30px;
            background-color: #8b5cf6;
            width: 15px;
            height: 15px;
            border-radius: 50%;
            opacity: 0.1;
          ""
        ></div>

        <p
          style=""
            font-size: 14px;
            color: #a78bfa;
            text-transform: uppercase;
            letter-spacing: 1.5px;
            margin: 0 0 10px 0;
            font-weight: 600;
          ""
        >
          Verification Code
        </p>
        <div
          style=""
            font-size: 12px;
            color: #f59e0b;
            font-weight: 700;
            letter-spacing: 5px;
            margin: 0;
            font-family: 'Courier New', monospace;
          ""
        >
          {verification}
        </div>
        <p style=""font-size: 12px; color: #9ca3af; margin-top: 15px"">
          This code will expire in 30 minutes
        </p>
      </div>
    </div>

    <!-- Action button -->
    <div style=""text-align: center; margin-bottom: 35px"">
      <a
        href=""#""
        style=""
          display: inline-block;
          background: linear-gradient(to right, #f59e0b, #f97316);
          color: #ffffff;
          font-weight: 600;
          padding: 14px 30px;
          border-radius: 8px;
          text-decoration: none;
          font-size: 16px;
          box-shadow: 0 4px 10px rgba(249, 115, 22, 0.3);
        ""
        >Verify My Account</a
      >
    </div>

    <!-- Security note -->
    <div
      style=""
        background-color: rgba(255, 255, 255, 0.03);
        border-radius: 8px;
        padding: 15px;
        margin-bottom: 30px;
        border: 1px solid rgba(255, 255, 255, 0.05);
      ""
    >
      <p style=""font-size: 14px; color: #9ca3af; margin: 0; line-height: 1.6"">
        <span style=""color: #f59e0b; font-weight: 600"">Security Note:</span> If
        you didn't request this email, please ignore it or contact our support
        team immediately. Your account security is important to us.
      </p>
    </div>
  </div>

  <!-- Footer -->
  <div
    style=""
      background-color: rgba(17, 24, 39, 0.8);
      padding: 30px;
      text-align: center;
      border-top: 1px solid rgba(255, 255, 255, 0.1);
    ""
  >
    <div style=""margin-bottom: 20px"">
      <span
        style=""
          display: inline-block;
          background: linear-gradient(to right, #8b5cf6, #f59e0b);
          -webkit-background-clip: text;
          -webkit-text-fill-color: transparent;
          font-size: 24px;
          font-weight: 800;
        ""
        >Trimly</span
      >
    </div>
    <p style=""font-size: 13px; color: #6b7280; margin: 0 0 15px 0"">
      This is an automated message, please do not reply directly to this email.
    </p>
    <div style=""margin-top: 20px"">
      <a
        href=""#""
        style=""
          color: #9ca3af;
          text-decoration: none;
          font-size: 13px;
          margin: 0 10px;
        ""
        >Help Center</a
      >
      <a
        href=""#""
        style=""
          color: #9ca3af;
          text-decoration: none;
          font-size: 13px;
          margin: 0 10px;
        ""
        >Privacy Policy</a
      >
      <a
        href=""#""
        style=""
          color: #9ca3af;
          text-decoration: none;
          font-size: 13px;
          margin: 0 10px;
        ""
        >Terms of Service</a
      >
    </div>
    <p style=""font-size: 12px; color: #4b5563; margin-top: 20px"">
      © 2025 Trimly. All rights reserved.
    </p>
  </div>
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
             <div
  style=""
    font-family: 'Segoe UI', Roboto, Arial, sans-serif;
    color: #f9fafb;
    line-height: 1.6;
    max-width: 600px;
    margin: 0 auto;
    border-radius: 16px;
    padding: 0;
    background-color: #111827;
    overflow: hidden;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
  ""
>
  <!-- Header with gradient -->
  <div
    style=""
      background: linear-gradient(
        135deg,
        #8b5cf6 0%,
        #6d28d9 50%,
        #4c1d95 100%
      );
      padding: 30px 20px;
      text-align: center;
    ""
  >
    <div
      style=""
        display: inline-block;
        background-color: rgba(255, 255, 255, 0.15);
        border-radius: 50px;
        padding: 8px 16px;
        margin-bottom: 20px;
      ""
    >
      <span
        style=""
          color: #ffffff;
          font-weight: 600;
          font-size: 14px;
          letter-spacing: 1px;
        ""
        >PASSWORD RECOVERY</span
      >
    </div>
    <h1
      style=""
        color: #ffffff;
        font-size: 32px;
        margin: 0 0 10px 0;
        font-weight: 800;
        text-align: center;
        letter-spacing: -0.5px;
      ""
    >
      Forgot Your Password?
    </h1>
    <p
      style=""
        color: rgba(255, 255, 255, 0.9);
        font-size: 16px;
        margin: 0;
        font-weight: 400;
      ""
    >
      No worries, we'll help you get back in
    </p>
  </div>

  <!-- Main content area with subtle gradient -->
  <div
    style=""
      background: linear-gradient(180deg, #1f2937 0%, #111827 100%);
      padding: 40px 30px;
    ""
  >
    <!-- Welcome message card -->
    <div
      style=""
        background-color: rgba(255, 255, 255, 0.05);
        border-left: 4px solid #f59e0b;
        border-radius: 8px;
        padding: 20px;
        margin-bottom: 30px;
      ""
    >
      <p style=""font-size: 17px; margin: 0; line-height: 1.7"">
        Hello
        <strong style=""color: #f97316; font-weight: 600"">{request.Email}</strong
        >,
        <br />
        We received a request to reset the password for your Trimly account.
        Don't worry - it happens to everyone!
      </p>
    </div>

    <!-- Password reset steps -->
    <div style=""margin-bottom: 30px"">
      <p
        style=""
          font-size: 16px;
          color: #f9fafb;
          margin-bottom: 15px;
          font-weight: 500;
        ""
      >
        Follow these simple steps to reset your password:
      </p>

      <div style=""display: flex; margin-bottom: 15px; align-items: flex-start"">
        <div
          style=""
            background-color: #8b5cf6;
            color: white;
            width: 24px;
            height: 24px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            margin-right: 15px;
            flex-shrink: 0;
          ""
        >
          1
        </div>
        <div style=""flex: 1"">
          <p style=""margin: 0; color: #e5e7eb; font-size: 15px"">
            Copy the verification code below
          </p>
        </div>
      </div>

      <div style=""display: flex; margin-bottom: 15px; align-items: flex-start"">
        <div
          style=""
            background-color: #8b5cf6;
            color: white;
            width: 24px;
            height: 24px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            margin-right: 15px;
            flex-shrink: 0;
          ""
        >
          2
        </div>
        <div style=""flex: 1"">
          <p style=""margin: 0; color: #e5e7eb; font-size: 15px"">
            Click the ""Reset Password"" button below to go to our secure password
            reset page
          </p>
        </div>
      </div>

      <div style=""display: flex; margin-bottom: 15px; align-items: flex-start"">
        <div
          style=""
            background-color: #8b5cf6;
            color: white;
            width: 24px;
            height: 24px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            margin-right: 15px;
            flex-shrink: 0;
          ""
        >
          3
        </div>
        <div style=""flex: 1"">
          <p style=""margin: 0; color: #e5e7eb; font-size: 15px"">
            Enter the verification code when prompted and create your new
            password
          </p>
        </div>
      </div>
    </div>

    <!-- Verification token box -->
    <div style=""text-align: center; margin-bottom: 35px"">
      <div
        style=""
          background: linear-gradient(145deg, #2d3748, #1e293b);
          border: 1px solid rgba(139, 92, 246, 0.3);
          border-radius: 12px;
          padding: 25px;
          position: relative;
          box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        ""
      >
        <!-- Decorative elements -->
        <div
          style=""
            position: absolute;
            top: -10px;
            left: 20px;
            background-color: #f59e0b;
            width: 20px;
            height: 20px;
            border-radius: 50%;
            opacity: 0.1;
          ""
        ></div>
        <div
          style=""
            position: absolute;
            bottom: -5px;
            right: 30px;
            background-color: #8b5cf6;
            width: 15px;
            height: 15px;
            border-radius: 50%;
            opacity: 0.1;
          ""
        ></div>

        <p
          style=""
            font-size: 14px;
            color: #a78bfa;
            text-transform: uppercase;
            letter-spacing: 1.5px;
            margin: 0 0 10px 0;
            font-weight: 600;
          ""
        >
          Your Verification Code
        </p>
        <div
          style=""
            font-size: 12px;
            color: #f59e0b;
            font-weight: 700;
            letter-spacing: 5px;
            margin: 0;
            font-family: 'Courier New', monospace;
          ""
        >
          {verification}
        </div>
        <p style=""font-size: 12px; color: #9ca3af; margin-top: 15px"">
          This code will expire in 30 minutes for security reasons
        </p>
      </div>
    </div>

    <!-- Action button -->
    <div style=""text-align: center; margin-bottom: 35px"">
      <a
        href=""#""
        style=""
          display: inline-block;
          background: linear-gradient(to right, #f59e0b, #f97316);
          color: #ffffff;
          font-weight: 600;
          padding: 14px 30px;
          border-radius: 8px;
          text-decoration: none;
          font-size: 16px;
          box-shadow: 0 4px 10px rgba(249, 115, 22, 0.3);
        ""
        >Reset Password</a
      >
    </div>

    <!-- Security note -->
    <div
      style=""
        background-color: rgba(255, 255, 255, 0.03);
        border-radius: 8px;
        padding: 15px;
        margin-bottom: 30px;
        border: 1px solid rgba(255, 255, 255, 0.05);
      ""
    >
      <p style=""font-size: 14px; color: #9ca3af; margin: 0; line-height: 1.6"">
        <span style=""color: #f59e0b; font-weight: 600"">Security Note:</span> If
        you didn't request to reset your password, you can safely ignore this
        email. Your account is still secure, and no changes have been made.
      </p>
    </div>

    <!-- Password tips -->
    <div
      style=""
        background-color: rgba(139, 92, 246, 0.05);
        border-radius: 8px;
        padding: 15px;
        margin-bottom: 30px;
        border: 1px solid rgba(139, 92, 246, 0.1);
      ""
    >
      <p
        style=""
          font-size: 14px;
          color: #a78bfa;
          font-weight: 600;
          margin: 0 0 10px 0;
        ""
      >
        Creating a Strong Password:
      </p>
      <ul
        style=""margin: 0; padding: 0 0 0 20px; color: #9ca3af; font-size: 13px""
      >
        <li style=""margin-bottom: 5px"">
          Use at least 8 characters with a mix of letters, numbers, and symbols
        </li>
        <li style=""margin-bottom: 5px"">
          Avoid using easily guessable information like birthdays
        </li>
        <li style=""margin-bottom: 0"">
          Consider using a unique password you don't use elsewhere
        </li>
      </ul>
    </div>
  </div>

  <!-- Footer -->
  <div
    style=""
      background-color: rgba(17, 24, 39, 0.8);
      padding: 30px;
      text-align: center;
      border-top: 1px solid rgba(255, 255, 255, 0.1);
    ""
  >
    <div style=""margin-bottom: 20px"">
      <span
        style=""
          display: inline-block;
          background: linear-gradient(to right, #8b5cf6, #f59e0b);
          -webkit-background-clip: text;
          -webkit-text-fill-color: transparent;
          font-size: 24px;
          font-weight: 800;
        ""
        >TRIMLY</span
      >
    </div>
    <p style=""font-size: 13px; color: #6b7280; margin: 0 0 15px 0"">
      Need help? Contact our support team at
      <a
        href=""mailto:support@trimly.com""
        style=""color: #a78bfa; text-decoration: none""
        >support@trimly.com</a
      >
    </p>
    <div style=""margin-top: 20px"">
      <a
        href=""#""
        style=""
          color: #9ca3af;
          text-decoration: none;
          font-size: 13px;
          margin: 0 10px;
        ""
        >Help Center</a
      >
      <a
        href=""#""
        style=""
          color: #9ca3af;
          text-decoration: none;
          font-size: 13px;
          margin: 0 10px;
        ""
        >Privacy Policy</a
      >
      <a
        href=""#""
        style=""
          color: #9ca3af;
          text-decoration: none;
          font-size: 13px;
          margin: 0 10px;
        ""
        >Terms of Service</a
      >
    </div>
    <p style=""font-size: 12px; color: #4b5563; margin-top: 20px"">
      © 2025 Trimly. All rights reserved.
    </p>
  </div>
</div>
                "    
            ,
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
            Username = user.UserName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            CreatedAt = user.CreateAt
        };
        return ApiResponse<AccountDto>.SuccessResponse(account);
    }

    public async Task<ApiResponse<UpdateAccountDto>> UpdateAccountDetailsAsync(UpdateAccountDto status, string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            return ApiResponse<UpdateAccountDto>.ErrorResponse("Account not found"); 
        }
        
        user.FirstName = status.FirstName;
        user.LastName = status.LastName;
        user.UserName = status.Username;
            
        var updateUser = await userManager.UpdateAsync(user);

        UpdateAccountDto accountDto = new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.UserName
        };
        return ApiResponse<UpdateAccountDto>.SuccessResponse(accountDto);
    }
    
    
    public async Task LogOutAsync()
    {
        await signInManager.SignOutAsync();
        
    }
    
    #region Private Methods

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
         
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken
        (
            issuer: _JwtSettings.Issuer,
            audience: _JwtSettings.Audience,
            claims: claim,
            expires: DateTime.Now.AddMinutes(_JwtSettings.DurationInMinutes),
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

    #endregion
   
}

