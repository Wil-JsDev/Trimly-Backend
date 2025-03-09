using Microsoft.AspNetCore.Http;

namespace Trimly.Core.Application.DTOs.RegisteredCompanies
{
    public sealed record CreateRegisteredCompaniesDTos
    (
        string? Name,
        string? Rnc,
        string? PhoneNumber,
        string? AddresCompanies,
        string? Email,
        string? DescriptionCompanies,
        IFormFile? ImageFile
    );
}
