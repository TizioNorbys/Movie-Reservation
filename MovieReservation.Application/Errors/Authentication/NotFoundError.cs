using FluentResults;

namespace MovieReservation.Application.Errors.Authentication;

public class NotFoundError : Error
{
	public NotFoundError(Type type)
		: base($"No {type.Name} entity was found")
	{
	}
}