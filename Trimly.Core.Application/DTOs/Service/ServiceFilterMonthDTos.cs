using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Application.DTOs.Service;

public record ServiceFilterMonthDTos
(       string? NameService,
        decimal Price,
        string? AboutService,
        int DurationInMinutes,
        ServiceStatus ServicesStatus
);