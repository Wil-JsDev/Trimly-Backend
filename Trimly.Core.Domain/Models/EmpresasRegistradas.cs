using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Models
{
    public sealed class EmpresasRegistradas
    {
        public Guid? Id { get; set; }

        public string? Nombre { get; set; }

        public string? RNC { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public string? Email { get; set; }

        public string? Descripcion { get; set; }

        public string? LogoUrl { get; set; }

        public Estado? Estado { get; set; }

        public DateTime? FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
