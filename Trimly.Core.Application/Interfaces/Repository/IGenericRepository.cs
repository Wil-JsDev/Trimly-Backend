using Trimly.Core.Application.Pagination;

namespace Trimly.Core.Application.Interfaces.Repository
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);

        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity);

        Task<PagedResult<TEntity>> GetPagedResult(int pageNumber,int pageSize, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
