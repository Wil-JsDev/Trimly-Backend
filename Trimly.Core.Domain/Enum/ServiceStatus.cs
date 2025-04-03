namespace Trimly.Core.Domain.Enum;

public enum ServiceStatus
{
    Pending, // The appointment is confirmed, but the service has not been provided yet  
    Completed // The service has been provided to the client  
}