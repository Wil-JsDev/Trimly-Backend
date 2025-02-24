using Trimly.Core.Domain.Common;

namespace Trimly.Core.Domain.Models
{
    public sealed class Reservations : CreationDateUpdate
    {
        public Guid? ReservationsId { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string? Note { get; set; }

        public string? ConfirmationCode { get; set; }

        public Guid? AppointmentId { get; set; }
        public ICollection<Appointments>? Appointments  { get; set; }
    }

}
