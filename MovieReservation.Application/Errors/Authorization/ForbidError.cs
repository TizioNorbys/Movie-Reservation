using FluentResults;

namespace MovieReservation.Application.Errors.Authorization;

public class ForbidError : Error
{
	public ForbidError()
		: base("Access denied")
	{
	}
}