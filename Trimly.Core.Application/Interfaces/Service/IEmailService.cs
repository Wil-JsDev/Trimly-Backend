
using Trimly.Core.Application.DTOs.Email;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IEmailService
    {
        Task SenAsync(EmailRequestDTos emailRequest);
    }
}
