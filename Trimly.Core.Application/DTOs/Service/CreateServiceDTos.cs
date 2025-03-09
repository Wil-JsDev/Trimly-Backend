
using Microsoft.AspNetCore.Http;

namespace Trimly.Core.Application.DTOs.Service
{
    public sealed record CreateServiceDTos
    (
        string? Name,
        decimal Price,
        string? Description,
        int DurationInMinutes,
        IFormFile? ImageFile,
        Guid? RegisteredCompanyId
    );
}
