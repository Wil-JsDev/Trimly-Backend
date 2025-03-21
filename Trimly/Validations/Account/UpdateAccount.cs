using FluentValidation;
using Trimly.Core.Application.DTOs.Account;

namespace Trimly.Presentation.Validations.Account;

public class UpdateAccount : AbstractValidator<UpdateAccountDto>
{
    public UpdateAccount()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters long.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(4).WithMessage("Username must be at least 4 characters long.");
    }
}