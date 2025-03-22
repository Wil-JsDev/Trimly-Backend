using FluentValidation;
using Trimly.Core.Application.DTOs.Account.Password;

namespace Trimly.Presentation.Validations.Account;

public class ResetPasswordAccount : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordAccount()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Enter a valid email address.");
        
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .MinimumLength(6).WithMessage("Confirm password must be at least 6 characters long.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}