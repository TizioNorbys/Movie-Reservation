using System.Linq.Expressions;

namespace MovieReservation.Domain.Exceptions;

public class InvalidPropertySelectorException : Exception
{
	public InvalidPropertySelectorException(Expression lambda, Type type)
		: base($"{lambda} does not select a property of {type.Name} type")
	{
	}
}