
namespace Trimly.Core.Application.DTOs.Review
{
    public sealed record ReviewsUpdateDTos
    (
        string? Title,
        string? Comment,
        int Rating
    );
}
