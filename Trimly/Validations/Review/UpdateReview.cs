using FluentValidation;
using Trimly.Core.Application.DTOs.Review;

namespace Trimly.Presentation.Validations.Review;

public class UpdateReview : AbstractValidator<ReviewsUpdateDTos>
{
    public UpdateReview()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The title is required.")
            .MaximumLength(50).WithMessage("The title cannot exceed 100 characters.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("The comment is required.")
            .MaximumLength(250).WithMessage("The comment cannot exceed 500 characters.");

        RuleFor(x => x.Rating)
            .NotEmpty().WithMessage("The rating is required.")
            .InclusiveBetween(1, 10).WithMessage("The rating must be between 1 and 10.");
    }
}