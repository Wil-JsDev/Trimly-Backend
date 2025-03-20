namespace Trimly.Core.Domain.Utils;

public static class AppointmentQueue
{
    private static readonly Queue<Guid> IdAppointment = new();
    public static Queue<Guid> Appointment => IdAppointment;

    public static void Add(Guid appointment)
    {
        IdAppointment.Enqueue(appointment);         
    }
    public static Guid Dequeue()
    {
       return IdAppointment.Dequeue();
    }
}