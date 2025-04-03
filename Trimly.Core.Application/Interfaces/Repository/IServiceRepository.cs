using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface IServiceRepository : IGenericRepository<Domain.Models.Services>
    {
        Task<IEnumerable<Domain.Models.Services>> GetServicesByNameAsync(Guid registeredCompaniesId, string name,CancellationToken cancellationToken);

        Task<IEnumerable<Domain.Models.Services>> GetServicesByPriceAsync(Guid registeredCompaniesId, decimal price, CancellationToken cancellationToken);

        Task<IEnumerable<Domain.Models.Services>> GetServicesByDurationInMinutesAsync(Guid registeredCompaniesId, int durationInMinutes, CancellationToken cancellationToken);
        
        Task<IEnumerable<Domain.Models.Services>> GetServicesWithDurationLessThan30MinutesAsync(Guid registeredCompaniesId, CancellationToken cancellationToken);

        Task<IEnumerable<Domain.Models.Services>> GetServicesByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);

        Task<IEnumerable<Domain.Models.Services>> GetCompletedServicesByMonthAsync(Guid registeredCompaniesId, int year, int month, CancellationToken cancellationToken);
    }
}
