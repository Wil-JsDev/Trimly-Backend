
namespace Trimly.Core.Application.DTOs.RegisteredCompanies
{
    public sealed record RegisteredCompaniesUpdateDTos
    (
        string? Name,
        string? Phone,
        string? Email,
        string? DescriptionCompanies,
        string? LogoUrl,
        string? AddressCompany,
        Domain.Enum.Status? Status
    );
}
