using System.Linq.Expressions;
using System.Threading;
using Trimly.Core.Application.Pagination;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);

        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task<PagedResult<TEntity>> GetPagedResultAsync(int pageNumber,int pageSize, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);

        Task<bool> ValidateAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
