using FluentValidation;
using Trimly.Core.Application.DTOs.Service;

namespace Trimly.Presentation.Validations.Services;

public class CreateService : AbstractValidator<CreateServiceDTos>
{
    public CreateService()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The name cannot be empty.")
            .MaximumLength(50).WithMessage("The name cannot exceed 50 characters.");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description cannot be empty.")
            .MaximumLength(100).WithMessage("Description cannot exceed 100 characters.");

        RuleFor(x => x.DurationInMinutes)
            .NotEmpty().WithMessage("Duration is required.");

        RuleFor(x => x.RegisteredCompanyId)
            .NotEmpty().WithMessage("Registered company ID is required.");
    }
}