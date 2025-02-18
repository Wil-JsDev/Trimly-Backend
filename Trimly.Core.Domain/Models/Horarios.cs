using Trimly.Core.Domain.Common;
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Models
{
    public sealed class Horarios : FechaCreacionActualizacion
    {
        public Guid? Id { get; set; }

        public DiaDeSemana Semana { get; set; }   

        public TimeOnly HoraDeApertura { get; set; }

        public TimeOnly HoraDeCierre { get; set; }

        public string? Notas { get; set; }

        public Estado? EsDiaFestivo { get; set; }

        public Guid? EmpresaRegistradaId { get; set; }

    }
}
