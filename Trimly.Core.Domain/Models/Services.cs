
using Trimly.Core.Domain.Common;
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Models
{
    public sealed class Services : CreationDateUpdate
    {
        public Guid? ServicesId { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int DurationInMinutes { get; set; }

        public string? ImageUrl { get; set; }

        public Status? Status { get; set; }

        public Guid? RegisteredCompanyId { get; set; }
    }
}
