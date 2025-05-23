using FluentValidation;
using MovieReservation.Application.DTOs.Review;

namespace MovieReservation.Application.Validators.Review;

public class CreateUpdateReviewDtoValidator : AbstractValidator<CreateUpdateReviewDto>
{
    public CreateUpdateReviewDtoValidator()
    {
        RuleFor(x => x.Rating)
            .NotEmpty()
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(10000);
    }
}