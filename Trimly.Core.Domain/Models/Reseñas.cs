using Trimly.Core.Domain.Common;

namespace Trimly.Core.Domain.Models
{
    public sealed class Reseñas : FechaCreacionActualizacion
    {
        public Guid? ReseñasId { get; set; }

        public string? Titulo {  get; set; }

        public string? Comentario { get; set; }

        public int Calificacion {  get; set; }

        public Guid? EmpresasRegistradasId { get; set; }
    }
}
