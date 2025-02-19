using Trimly.Core.Domain.Common;
using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Models
{
    public sealed class Appointments : CreationDateUpdate
    {
        public Guid? AppointmentId { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public AppointmentStatus? AppointmentStatus { get; set; }

        public string? ConfirmationCode { get; set; }

        public string? CancellationReason { get; set; }

        public Guid? ServiceId { get; set; }

    }
}
