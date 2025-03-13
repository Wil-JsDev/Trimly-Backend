
using Trimly.Core.Application.DTOs.Email;

namespace Trimly.Core.Application.Interfaces.Service
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDTos emailRequest);
    }
}
