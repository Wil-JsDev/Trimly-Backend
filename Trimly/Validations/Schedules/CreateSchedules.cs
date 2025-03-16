using FluentValidation;
using Trimly.Core.Application.DTOs.Schedules;

namespace Trimly.Presentation.Validations.Schedules;

public class CreateSchedules : AbstractValidator<CreateSchedulesDTos>
{
    public CreateSchedules()
    {
        RuleFor(x => x.Days)
            .NotEmpty().WithMessage("The day(s) field is required.")
            .IsInEnum().WithMessage("The selected day is not valid.");

        RuleFor(x => x.OpeningTime)
            .NotEmpty().WithMessage("The opening time is required.");

        RuleFor(x => x.ClosingTime)
            .NotEmpty().WithMessage("The closing time is required.")
            .GreaterThan(x => x.OpeningTime).WithMessage("The closing time must be later than the opening time.");

        RuleFor(x => x.Notes)
            .NotEmpty().WithMessage("The notes field cannot be empty.")
            .MaximumLength(150).WithMessage("The notes field cannot exceed 500 characters.");

        RuleFor(x => x.RegisteredCompanyId)
            .NotEmpty().WithMessage("The registered company ID is required.")
            .NotEqual(Guid.Empty).WithMessage("The registered company ID must be a valid GUID.");
    }
}