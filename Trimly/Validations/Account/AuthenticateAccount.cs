using FluentValidation;
using Trimly.Core.Application.DTOs.Account.Authenticate;

namespace Trimly.Presentation.Validations.Account;

public class AuthenticateAccount : AbstractValidator<AuthenticateRequest>
{
    public AuthenticateAccount()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Enter a valid email address.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 2 characters long.");
    }
}