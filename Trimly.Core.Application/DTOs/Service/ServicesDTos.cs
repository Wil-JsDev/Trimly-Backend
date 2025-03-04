
namespace Trimly.Core.Application.DTOs.Service
{
    public sealed record ServicesDTos
    (
        Guid? ServiceId,
        string? Name,
        decimal Price,
        string? Description,
        int DurationInMinutes,
        string? ImageUrl,
        Domain.Enum.Status? Status,
        Guid? RegisteredCompanyId,
        DateTime? CreatedAt,
        DateTime? UpdateAt,
        decimal PenaltyAmount
    );
}
