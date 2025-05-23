using FluentResults;

namespace MovieReservation.Application.Errors.Reservation;

public class NoSeatsError : Error
{
	public NoSeatsError()
		: base("No seats selected")
	{
	}
}