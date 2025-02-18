
namespace Trimly.Core.Domain.Common
{
    public class FechaCreacionActualizacion
    {
        public DateTime? FechaDeCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaDeActualizacion { get; set; }
    }
}
