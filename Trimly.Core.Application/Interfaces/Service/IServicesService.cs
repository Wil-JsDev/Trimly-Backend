using Trimly.Core.Application.DTOs.Service;
using Trimly.Core.Domain.Enum;
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
        
        Task<ResultT<IEnumerable<ServicesDTos>>> GetServicesByDurationMinutesAsync(Guid registeredCompaniesId, int durationInMinutes, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<ServiceFilterMonthDTos>>> GetServicesByMonthAsync(Guid registeredCompanyId, int year, MonthFilter month, CancellationToken cancellationToken);
    }
}
