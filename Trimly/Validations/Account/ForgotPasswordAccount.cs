using FluentValidation;
using Trimly.Core.Application.DTOs.Account.Password;

namespace Trimly.Presentation.Validations.Account;

public class ForgotPasswordAccount : AbstractValidator<ForgotRequest>
{
    public ForgotPasswordAccount()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Enter a valid email address.");
    }
}