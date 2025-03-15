using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Trimly.Core.Application.DTOs.Email;
using Trimly.Core.Application.Interfaces.Service;

namespace Trimly.Core.Application.Services;

public class CodeConfirmationService : ICodeConfirmationService
{
    private static readonly ConcurrentDictionary<string, string> _generatedCodes = new();
    private readonly IEmailService _emailService;

    public CodeConfirmationService(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task<string> GenerateCodeConfirmation(string emailAddress)
    {
         const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        int length = RandomNumberGenerator.GetInt32(8, 16);
        StringBuilder stringBuilder = new(length);

        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(chars[RandomNumberGenerator.GetInt32(chars.Length)]);
        }

        string randomString = stringBuilder.ToString();

        // Garantizar unicidad por usuario
        _generatedCodes[emailAddress] = randomString;

        await _emailService.SendAsync(new EmailRequestDTos
        {
            To = emailAddress,
            Body = $@"
                <div style='font-family: Arial, sans-serif; color: #333; line-height: 1.8; max-width: 600px; margin: 0 auto; border: 1px solid #e6e6e6; border-radius: 10px; padding: 25px; background-color: #ffffff;'>
                <h1 style='color: #007BFF; font-size: 26px; margin-bottom: 20px; text-align: center;'>Confirm Your Account Registration</h1>
                <p style='font-size: 16px; margin-bottom: 20px; text-align: center;'>
                    Hello <strong>{emailAddress}</strong>, <br>
                    Thank you for registering with us. To complete your account setup, please use the verification token provided below.
                </p>
                <div style='font-size: 16px; background-color: #f9f9f9; padding: 20px; border: 1px dashed #ccc; border-radius: 8px; margin-bottom: 20px; text-align: center;'>
                <strong style='color: #333;'>Your Verification Token:</strong>
                <p style='font-size: 18px; color: #007BFF; font-weight: bold;'>{randomString}</p>
                </div>
                <p style='font-size: 14px; margin-bottom: 20px; text-align: center;'>
                If you didn’t request this email, no further action is required. Please feel free to contact us if you have any concerns.
                </p>
                <hr style='border: none; border-top: 1px solid #e6e6e6; margin: 30px 0;'>
                <p style='font-size: 12px; color: #888; text-align: center;'>
                This email is brought to you by <strong>Caring for Paws</strong>. <br>
                Please do not reply directly to this email as it is not monitored.
                </p>
                </div>",
            Subject = "Confirmation Code"
        });

        return randomString;
    }
}