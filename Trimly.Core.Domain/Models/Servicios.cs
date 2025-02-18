
using Trimly.Core.Domain.Common;
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Models
{
    public sealed class Servicios : FechaCreacionActualizacion
    {
        public Guid? Id { get; set; }

        public string? Nombre { get; set; }

        public Decimal Precio { get; set; }

        public string? Descripcion { get; set; }

        public int DuracionEnMinutos { get; set; }

        public string? ImagenUrl { get; set; }

        public Estado? Estado { get; set; }

        public Guid? EmpresasRegistradasId { get; set; }

    }
}
