using FluentValidation;
using Trimly.Core.Application.DTOs.Appointment;

namespace Trimly.Presentation.Validations.Appointment;

public class CreateAppointment : AbstractValidator<CreateAppointmentDTos>
{
    public CreateAppointment()
    {
        RuleFor(x => x.StarDateTime)
            .NotEmpty().WithMessage("The start date and time is required.");

        RuleFor(x => x.EndDateTime)
            .NotEmpty().WithMessage("The end date and time is required.");

        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("The service ID is required.")
            .NotEqual(Guid.Empty).WithMessage("The service ID must be a valid GUID.");
    }
}