
using Trimly.Core.Domain.Common;
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Models
{
    public sealed class Services : CreationDateUpdate
    {
        public Guid? ServicesId { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public decimal PenaltyAmount { get; set; }
        
        public string? Description { get; set; }

        public int DurationInMinutes { get; set; }
        
        public ServiceStatus ServiceStatus { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        public string? ImageUrl { get; set; }

        public Status? Status { get; set; }

        public Guid? RegisteredCompanyId { get; set; }
        public RegisteredCompanies? RegisteredCompanies { get; set; }
        public ICollection<Appointments>? Appointments { get; set; }
    }
}
