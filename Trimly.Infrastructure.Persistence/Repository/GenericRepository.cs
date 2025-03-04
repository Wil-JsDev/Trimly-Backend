using Microsoft.EntityFrameworkCore;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Application.Pagination;
using Trimly.Infrastructure.Persistence.Context;

namespace Trimly.Infrastructure.Persistence.Repository
{
    public class GenericRepository<T>: IGenericRepository<T> where T : class
    {
        protected readonly TrimlyContext _context;

        public GenericRepository(TrimlyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            _context.Set<T>().Add(entity);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            _context.Set<T>().Remove(entity);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken) => await _context.Set<T>().FindAsync(id, cancellationToken);

        public async Task<PagedResult<T>> GetPagedResultAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var total = await _context.Set<T>().AsNoTracking().CountAsync(cancellationToken);

            var entity = await _context.Set<T>().AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>(entity, total, pageNumber, pageSize);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync();

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync(cancellationToken);
        }
    }
}
