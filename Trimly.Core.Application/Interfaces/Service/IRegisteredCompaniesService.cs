using Trimly.Core.Application.DTOs.RegisteredCompanies;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Utils;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IRegisteredCompaniesService : IGenericService<
        CreateRegisteredCompaniesDTos,
        RegisteredCompaniesUpdateDTos,
        RegisteredCompaniesDTos>
    {
        Task<ResultT<IEnumerable<OrderNameComapanyDTos>>> FilterByNameAsync(string name, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<RegisteredCompaniesDTos>>> FilterByStatusAsync(Status status, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<RegisteredCompaniesDTos>>> OrderByNameAsync(CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<RegisteredCompaniesDTos>>> OrderByIdAsync(string order, CancellationToken cancellationToken);

        Task<ResultT<IEnumerable<RegisteredCompaniesDTos>>> GetRecentAsync(CancellationToken cancellationToken);
    }
}
