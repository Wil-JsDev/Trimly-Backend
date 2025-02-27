using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface IReviewsRepository : IGenericRepository<Reviews>
    {
        Task<double> GetAverageRatingAsync(Guid registeredCompanyId,CancellationToken cancellationToken);
    }
}
