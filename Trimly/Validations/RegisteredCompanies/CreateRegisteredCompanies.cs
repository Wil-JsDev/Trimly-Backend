using FluentValidation;
using Trimly.Core.Application.DTOs.RegisteredCompanies;

namespace Trimly.Presentation.Validations.RegisteredCompanies;

public class CreateRegisteredCompanies : AbstractValidator<CreateRegisteredCompaniesDTos>
{
    public CreateRegisteredCompanies()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The file name is required.")
            .MaximumLength(50).WithMessage("The file name must not exceed 50 characters.");

        RuleFor(x => x.Rnc)
            .NotEmpty().WithMessage("The RNC is required.")
            .Matches(@"^\d+$").WithMessage("The RNC must contain only numbers.")
            .Length(9, 11).WithMessage("The RNC must be between 9 and 11 digits.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("The phone number is required.");

        RuleFor(x => x.AddresCompanies)
            .NotEmpty().WithMessage("The company address is required.")
            .MaximumLength(200).WithMessage("The company address must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("The email is required.")
            .EmailAddress().WithMessage("The email format is invalid.");

        RuleFor(x => x.DescriptionCompanies)
            .NotEmpty().WithMessage("The company description is required.")
            .MinimumLength(10).WithMessage("The company description must be at least 10 characters long.")
            .MaximumLength(250).WithMessage("The company description must not exceed 250 characters.");
        
    }
}