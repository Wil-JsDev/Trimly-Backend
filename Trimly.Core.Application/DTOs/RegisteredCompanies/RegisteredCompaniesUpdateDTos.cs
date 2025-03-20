
using Microsoft.AspNetCore.Http;

namespace Trimly.Core.Application.DTOs.RegisteredCompanies
{
    public sealed record RegisteredCompaniesUpdateDTos
    (
        string? Name,
        string? Phone,
        string? Email,
        string? DescriptionCompanies,
        IFormFile? ImageFile,
        string? AddressCompany,
        Domain.Enum.Status? Status
    );
}
