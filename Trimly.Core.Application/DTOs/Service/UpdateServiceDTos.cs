
namespace Trimly.Core.Application.DTOs.Service
{
    public sealed record UpdateServiceDTos
    (
        string? Name,
        decimal Price,
        string? Description,
        int DurationInMinutes
    );
}
