using System.Collections.Concurrent;

namespace Trimly.Core.Domain.Utils;

public class AppointmentQueue
{
    private readonly ConcurrentQueue<Guid> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);

    public void Enqueue(Guid appointmentId)
    {
        _queue.Enqueue(appointmentId);
        _signal.Release(); // Notify that there is a new ID in the queue
    }
    
    public async Task<Guid?> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);

        if (_queue.TryDequeue(out var appointmentId))
        {
            return appointmentId;
        }
        return null;
    }
}