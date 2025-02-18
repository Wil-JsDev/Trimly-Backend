using Trimly.Core.Domain.Common;
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Models
{
    public sealed class Citas : FechaCreacionActualizacion
    {
        public Guid? Id { get; set; }

        public DateTime? FechaHoraInicio { get; set; }

        public DateTime? FechaHoraFin { get; set; }

        public EstadoCitas? EstadoCitas {  get; set; }

        public string? CodigoConfirmacion { get; set; }

        public string? MotivoCancelacion { get; set; }

        public Guid? ServicioId { get; set; }
    }
}
