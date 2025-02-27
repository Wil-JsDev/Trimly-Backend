using Trimly.Core.Application.DTOs.RegisteredCompanies;
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IRegisteredCompaniesService : IGenericService<
        CreateRegisteredCompaniesDTos,
        RegisteredCompaniesUpdateDTos,
        RegisteredCompaniesDTos>
    {
        Task<IEnumerable<RegisteredCompaniesDTos>> FilterByNameAsync(string name, CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompaniesDTos>> FilterByStatus(Status status, CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompaniesDTos>> FilterByRncAsync(string rnc,CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompaniesDTos>> OrderByNameAsync(CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompaniesDTos>> OrderByIdAsync(string order,CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompaniesDTos>> GetRecentAsync(CancellationToken cancellationToken);
    }
}
