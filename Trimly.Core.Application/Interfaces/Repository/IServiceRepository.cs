using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface IServiceRepository : IGenericRepository<Services>
    {
        Task<IEnumerable<Services>> GetServicesByNameAsync(string name, Guid registeredCompaniesId,CancellationToken cancellationToken);

        Task<IEnumerable<Services>> GetServicesByPriceAsync(double price, Guid registeredCompaniesId, CancellationToken cancellationToken);

        Task ApplyDiscountCodeAsync(Services services, Guid registeredCompaniesId, int discount, string discountCode ,CancellationToken cancellationToken);

        Task<IEnumerable<Services>> GetServicesWithDurationLessThan30MinutesAsync(Guid registeredCompaniesId, CancellationToken cancellationToken);
    }
}
