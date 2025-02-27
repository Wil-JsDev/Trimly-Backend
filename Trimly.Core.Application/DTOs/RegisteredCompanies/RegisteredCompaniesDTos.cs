
namespace Trimly.Core.Application.DTOs.RegisteredCompanies
{
    public sealed record RegisteredCompaniesDTos
    (
        Guid? RegisteredCompaniesId,
        string? Name,
        string? PhoneNumber,
        string? AddresCompanies,
        string? DescriptionCompanies,
        string? LogoUrl,
        Domain.Enum.Status? Status  
    );
}
