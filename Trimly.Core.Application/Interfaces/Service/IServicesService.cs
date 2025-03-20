using Trimly.Core.Application.DTOs.Service;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IServicesService : IGenericService<
        CreateServiceDTos,
        UpdateServiceDTos,
        ServicesDTos>
    {
        Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesWithDurationLessThan30MinutesAsync(Guid registeredCompany, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<ServicesDTos>>> GetServicesByCompanyIdAsync(Guid registeredCompaniesId, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesByNameAsync(string name, Guid registeredCompanyId, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<ServiceFilterDTos>>> GetServicesByPriceAsync(decimal price, Guid registeredCompanyId, CancellationToken cancellationToken);

    }
}
