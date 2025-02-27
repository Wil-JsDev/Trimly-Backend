using Trimly.Core.Application.DTOs.Review;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IReviewService : IGenericService<
        CreateReviewsDTos,
        ReviewsUpdateDTos,
        ReviewsDTos>
    {
        Task<double> GetAverageRatingAsync(Guid companyId,CancellationToken cancellationToken);
    }
}
