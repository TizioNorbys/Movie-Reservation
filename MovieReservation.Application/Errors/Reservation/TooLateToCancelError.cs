using FluentResults;

namespace MovieReservation.Application.Errors.Reservation;

public class TooLateToCancelError : Error
{
	public TooLateToCancelError()
		: base("Reservation can be cancelled up to 12 hours before the showtime")
	{
	}
}