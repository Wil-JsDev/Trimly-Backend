
namespace Trimly.Core.Application.DTOs.Review
{
    public sealed record ReviewsDTos
    (
        Guid? ReviewId,
        string? Title,
        string? Comment,
        int Rating,
        Guid? RegisteredCompanyId,
        DateTime? CreatedAt,
        DateTime? UpdateAt
    );
}
