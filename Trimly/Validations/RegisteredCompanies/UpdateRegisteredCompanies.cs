using System.Data;
using FluentValidation;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.DTOs.RegisteredCompanies;

namespace Trimly.Presentation.Validations.RegisteredCompanies;

public class UpdateRegisteredCompanies : AbstractValidator<RegisteredCompaniesUpdateDTos>
{
    public UpdateRegisteredCompanies()
    {
      
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The file name is required.")
            .MaximumLength(50).WithMessage("The file name must not exceed 50 characters.");


        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("The phone number is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("The email is required.")
            .EmailAddress().WithMessage("The email format is invalid.");

        RuleFor(x => x.DescriptionCompanies)
            .NotEmpty().WithMessage("The company description is required.")
            .MinimumLength(10).WithMessage("The company description must be at least 10 characters long.")
            .MaximumLength(250).WithMessage("The company description must not exceed 250 characters.");

        RuleFor(x => x.AddressCompany)
            .NotEmpty().WithMessage("The company address is required.")
            .MaximumLength(200).WithMessage("The company address must not exceed 200 characters.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("The status is required.")
            .IsInEnum().WithMessage("The status must be a valid value.");


    }
}