using Trimly.Core.Domain.Common;

namespace Trimly.Core.Domain.Models
{
    public sealed class Reservaciones : FechaCreacionActualizacion
    {
        public Guid? Id { get; set; }

        public DateTime? FechaHoraInicio { get; set; }

        public DateTime? FechaHoraFin  { get; set; }

        public string? Nota { get; set; }

        public string? CodigoDeConfirmacion { get; set; }

        public Guid? CitasId { get; set; }

    }
}
