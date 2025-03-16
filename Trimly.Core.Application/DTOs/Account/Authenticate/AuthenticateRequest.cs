namespace Trimly.Core.Application.DTOs.Account.Authenticate;

public class AuthenticateRequest
{
    public string? Email { get; set; }
    public string? Password {get; set; }
}