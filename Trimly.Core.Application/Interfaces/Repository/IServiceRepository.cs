using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface IServiceRepository : IGenericRepository<Services>
    {
        Task<IEnumerable<Services>> GetServicesByNameAsync(Guid registeredCompaniesId, string name,CancellationToken cancellationToken);

        Task<IEnumerable<Services>> GetServicesByPriceAsync(Guid registeredCompaniesId, decimal price, CancellationToken cancellationToken);

        Task<IEnumerable<Services>> GetServicesByDurationInMinutesAsync(Guid registeredCompaniesId, int durationInMinutes, CancellationToken cancellationToken);

        Task ApplyDiscountCodeAsync(Services services, Guid registeredCompaniesId, int discount, string discountCode ,CancellationToken cancellationToken);

        Task<IEnumerable<Services>> GetServicesWithDurationLessThan30MinutesAsync(Guid registeredCompaniesId, CancellationToken cancellationToken);

    }
}
