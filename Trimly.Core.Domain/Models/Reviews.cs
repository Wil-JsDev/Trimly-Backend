using Trimly.Core.Domain.Common;

namespace Trimly.Core.Domain.Models
{
    public sealed class Reviews : CreationDateUpdate
    {
        public Guid? ReviewId { get; set; }

        public string? Title { get; set; }

        public string? Comment { get; set; }

        public int Rating { get; set; }

        public Guid? RegisteredCompanyId { get; set; }
    }
}
