using System.Linq.Expressions;

namespace MovieReservation.Domain.Exceptions;

public class InvalidSelectorExpressionException : Exception
{
	public InvalidSelectorExpressionException(Expression lambda)
		: base($"{lambda} is not a valid selector expression")
	{
	}
}