
namespace Trimly.Core.Application.DTOs.Review
{
    public sealed record CreateReviewsDTos
    (
        string? Title,
        string? Comment,
        int Rating,
        Guid? RegisteredCompanyId
    );
}
