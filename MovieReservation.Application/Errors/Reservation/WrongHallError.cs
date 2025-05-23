using FluentResults;

namespace MovieReservation.Application.Errors.Reservation;

public class WrongHallError : Error
{
	public WrongHallError()
		: base("Selected seat is not in the showtime's hall")
	{
	}
}