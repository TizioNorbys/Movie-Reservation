using FluentValidation;
using MovieReservation.Application.Constants;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Domain.Constants;

namespace MovieReservation.Application.Validators.Movie;

public class CreateUpdateMovieDtoValidator : AbstractValidator<CreateUpdateMovieDto>
{
	public CreateUpdateMovieDtoValidator()
	{
		RuleFor(x => x.Title)
			.NotEmpty()
			.MaximumLength(50);

		RuleFor(x => x.Description)
			.NotEmpty()
			.MaximumLength(300);

		RuleFor(x => x.Plot)
			.NotEmpty()
			.MaximumLength(3000);

		RuleFor(x => x.ReleaseYear)
			.NotEmpty()
			.InclusiveBetween(1901, 2155);

		RuleFor(x => x.OriginalLanguage)
			.NotEmpty()
			.MaximumLength(20)
            .Must(lang => Constant<Languages>.IsDefined(lang))
                .WithMessage("Movie language is not valid");

		RuleFor(x => x.Country)
			.NotEmpty()
			.MaximumLength(30)
            .Must(country => Constant<Countries>.IsDefined(country))
                .WithMessage("Movie country is not valid");

		RuleFor(x => x.RunningTime)
			.NotEmpty()
			.GreaterThan(0);

		RuleFor(x => x.Rating)
			.NotEmpty()
			.InclusiveBetween(1, 10);

		RuleFor(x => x.Genres)
			.NotEmpty().WithMessage("Movies must have at least one gender");
		RuleForEach(x => x.Genres)
			.NotEmpty()
			.Must(genre => Constant<MovieGenres>.IsDefined(genre))
				.WithMessage("Movie genre is not valid");

	}
}