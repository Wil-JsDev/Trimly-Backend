using Trimly.Core.Application.DTOs.Service;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IServicesService : IGenericService<
        CreateServiceDTos,
        UpdateServiceDTos,
        ServicesDTos>
    {
        Task ApplyDiscountCodeAsync(Guid serviceId,Guid registeredCompanyId, double discount, string discountCode, CancellationToken cancellationToken);

        Task<IEnumerable<ServiceFilterDTos>> GetServicesWithDurationLessThan30MinutesAsync(Guid registeredCompany, CancellationToken cancellationToken);

        Task<IEnumerable<ServicesDTos>> GetServicesByCompanyIdAsync(Guid registeredCompaniesId, CancellationToken cancellationToken);

        Task<IEnumerable<ServiceFilterDTos>> GetServicesByNameAsync(string name, Guid registeredCompanyId, CancellationToken cancellationToken);

        Task<IEnumerable<ServiceFilterDTos>> GetServicesByPriceAsync(double price, Guid registeredCompanyId, CancellationToken cancellationToken);
    }
}
