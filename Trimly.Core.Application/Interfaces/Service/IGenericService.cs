using Trimly.Core.Application.Pagination;
namespace Trimly.Core.Application.Interfaces.Service
{
    /// <summary>
    /// Generic interface for managing entities in the application.
    /// Provides basic CRUD operations.
    /// </summary>
    /// <typeparam name="TCreateDTO">DTO for entity creation.</typeparam>
    /// <typeparam name="TUpdateDTO">DTO for entity update.</typeparam>
    /// <typeparam name="TResponseDTO">DTO for entity response.</typeparam>
    public interface IGenericService <TCreateDTo,TUpdateDTo,TResponseDTo>
    {
        Task<PagedResult<TResponseDTo>> GetPagedResult(int pageNumber,int pageSize,CancellationToken cancellationToken);

        Task<TResponseDTo> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<TResponseDTo> CreateAsync(TCreateDTo entity, CancellationToken cancellationToken);

        Task<TResponseDTo> UpdateAsync(Guid id, TUpdateDTo entity,CancellationToken cancellation);

        Task<Guid> DeleteAsync(Guid id, CancellationToken cancellation);
    }
}
