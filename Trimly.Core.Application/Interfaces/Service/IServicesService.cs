using Trimly.Core.Application.DTOs.Service;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IServicesService : IGenericService<
        CreateServiceDTos,
        UpdateServiceDTos,
        ServicesDTos>
    {
        Task<ResultT<Guid>> ApplyDiscountCodeAsync(Guid serviceId,Guid registeredCompanyId, double discount, string discountCode, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesWithDurationLessThan30MinutesAsync(Guid registeredCompany, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<ServicesDTos>>> GetServicesByCompanyIdAsync(Guid registeredCompaniesId, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesByNameAsync(string name, Guid registeredCompanyId, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesByPriceAsync(double price, Guid registeredCompanyId, CancellationToken cancellationToken);
    }
}
