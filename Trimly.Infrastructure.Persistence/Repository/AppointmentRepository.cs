using Microsoft.EntityFrameworkCore;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;
using Trimly.Infrastructure.Persistence.Context;

namespace Trimly.Infrastructure.Persistence.Repository
{
    public class AppointmentRepository : GenericRepository<Appointments>, IAppointmentRepository
    {
        public AppointmentRepository(TrimlyContext context) : base(context){}

        

        public async Task CancelAppointmentAsync(Appointments appointments, CancellationToken cancellationToken)
        {
            appointments.AppointmentStatus = AppointmentStatus.Cancelled;
            _context.Update(appointments);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task CancelAppointmentWithPenaltyAsync(Appointments appointments, double penalizationPorcentage, CancellationToken cancellationToken)
        {
            appointments.AppointmentStatus = AppointmentStatus.Cancelled;
            appointments.Services.PenaltyAmount = appointments.Services.Price * (decimal) (penalizationPorcentage / 100);
            appointments.Services.Price += appointments.Services.PenaltyAmount;
            _context.Update(appointments);
            await SaveChangesAsync(cancellationToken);

        }

        public async Task ConfirmAppointmentAutomaticallyAsync(Appointments appointments , CancellationToken cancellationToken)
        {
            appointments.AppointmentStatus = AppointmentStatus.Confirmed;
            await SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Appointments>> FilterByStatusAsync(AppointmentStatus appointmentStatus, CancellationToken cancellationToken) => 
            await _context.Set<Appointments>().AsNoTracking()
            .Where(s => s.AppointmentStatus == appointmentStatus)
            .ToListAsync(cancellationToken);
            

        public async Task<int> GetTotalAppointmentCountAsync(Guid serviceId, CancellationToken cancellationToken) => 
            await _context.Set<Appointments>()
            .Where(t => t.ServiceId == serviceId)
            .CountAsync(cancellationToken);

        public async Task RescheduleAppointmentAsync(Appointments appointment, CancellationToken cancellationToken)
        {
            _context.Update(appointment);
            await SaveChangesAsync(cancellationToken);
        }

    }
}
