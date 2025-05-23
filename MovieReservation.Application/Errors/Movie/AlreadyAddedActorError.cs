using FluentResults;

namespace MovieReservation.Application.Errors.Movie;

public class AlreadyAddedActorError : Error
{
	public AlreadyAddedActorError()
		: base("The actor has already been added to the film")
	{
	}
}