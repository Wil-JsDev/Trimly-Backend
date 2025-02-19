using Trimly.Core.Domain.Common;
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Models
{
    public sealed class Schedules : CreationDateUpdate
    {
        public Guid? SchedulesId { get; set; }

        public Weekday Week { get; set; }

        public TimeOnly OpeningTime { get; set; }

        public TimeOnly ClosingTime { get; set; }

        public string? Notes { get; set; }

        public Status? IsHoliday { get; set; }

        public Guid? RegisteredCompanyId { get; set; }
    }
}
