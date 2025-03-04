
namespace Trimly.Core.Application.DTOs.Service
{
    public sealed record ServiceFilterDTos
    (
        string? Name,
        decimal Price,
        string? Description,
        int DurationInMinutes,
        string? ImageUrl,
        Guid? RegisteredCompanyId,
        DateTime? CreatedAt,
        DateTime? UpdateAt
    );
}
