using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Models
{
    public sealed class RegisteredCompanies
    {
        public Guid? RegisteredCompaniesId { get; set; }

        public string? Name { get; set; }

        public string? RNC { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public string? Description { get; set; }

        public string? LogoUrl { get; set; }

        public Status? Status { get; set; }

        public DateTime? RegistrationDate { get; set; } = DateTime.UtcNow;
        public ICollection<Services>? Services { get; set; }
        public ICollection<Reviews>? Reviews { get; set; }
        public ICollection<Schedules>? Schedules { get; set; }
    }
}
