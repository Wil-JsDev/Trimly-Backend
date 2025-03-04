using Trimly.Core.Application.DTOs.Review;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IReviewService : IGenericService<
        CreateReviewsDTos,
        ReviewsUpdateDTos,
        ReviewsDTos>
    {
        Task<ResultT<double>> GetAverageRatingAsync(Guid companyId,CancellationToken cancellationToken);
    }
}
