using FluentValidation;
using MovieReservation.Application.DTOs.Showtime;

namespace MovieReservation.Application.Validators.Showtime;

public class CreateUpdateShowtimeDtoValidator : AbstractValidator<ICreateUpdateShowtimeDto>
{
	public CreateUpdateShowtimeDtoValidator()
	{
		RuleFor(x => x.Timestamp)
			.GreaterThan(DateTime.UtcNow);
	}
}