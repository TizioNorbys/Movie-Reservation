using FluentResults;

namespace MovieReservation.Application.Errors.Review;

public class AlreadyReactedError : Error
{
	public AlreadyReactedError()
		: base("User has already reacted to the review")
	{
	}
}