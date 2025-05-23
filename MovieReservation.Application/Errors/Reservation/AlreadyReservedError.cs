using FluentResults;

namespace MovieReservation.Application.Errors.Reservation;

public class AlreadyReservedError : Error
{
	public AlreadyReservedError(int seatNumber)
		: base($"Seat {seatNumber} is already reserved")
	{
	}
}