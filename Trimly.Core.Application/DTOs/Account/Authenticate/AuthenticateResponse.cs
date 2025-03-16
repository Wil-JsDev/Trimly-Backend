using System.Text.Json.Serialization;

namespace Trimly.Core.Application.DTOs.Account.Authenticate;

public class AuthenticateResponse
{
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public List<string>? Roles { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsVerified { get; set; }
    public int StatusCode { get; set; }
    public string? JwtToken { get; set; }
    [JsonIgnore]
    public string? RefreshToken { get; set; }
}