using System.Linq.Expressions;

namespace MovieReservation.Domain.Exceptions;

public class InvalidNavigationPropertySelectorException : Exception
{
    public InvalidNavigationPropertySelectorException(LambdaExpression lambda, Type type)
        : base($"{lambda} does not select a navigation property of {type.Name} type")
    {
    }
}