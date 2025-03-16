namespace Trimly.Core.Application.DTOs.RegisteredCompanies;

public record ApplyDiscountCodeRequest(
    Guid ServiceId,
    Guid RegisteredCompanyId,
    string DiscountCode);