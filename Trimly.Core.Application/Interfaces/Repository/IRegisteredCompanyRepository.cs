using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface IRegisteredCompanyRepository : IGenericRepository<RegisteredCompanies>
    {
        Task<RegisteredCompanies> FilterByNameAsync(string nameCompany,CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompanies>> FilterByStatusAsync(Status status,CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompanies>> OrderByNameAsync(CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompanies>> OrderByIdDescAsync(CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompanies>> OrderByIdAscAsync(CancellationToken cancellationToken);

        Task<IEnumerable<RegisteredCompanies>> GetRecentAsync(CancellationToken cancellationToken);
    }
}
