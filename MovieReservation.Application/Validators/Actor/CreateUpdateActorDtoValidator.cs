using FluentValidation;
using Microsoft.Extensions.Options;
using MovieReservation.Application.DTOs.Actor;
using MovieReservation.Application.Options;

namespace MovieReservation.Application.Validators.Actor;

public class CreateUpdateActorDtoValidator : AbstractValidator<CreateUpdateActorDto>
{
	public CreateUpdateActorDtoValidator(IOptions<DateOnlyOptions> options)
	{
		RuleFor(x => x.FullName)
			.NotEmpty()
			.MaximumLength(60);

		RuleFor(x => x.BirthDate)
			.NotEmpty()
			.InclusiveBetween(options.Value.Min, DateOnly.FromDateTime(DateTime.UtcNow));
	}
}