using FluentResults;
using FluentValidation.Results;

namespace MovieReservation.Application.Errors.Authentication;

public class ValidationError : Error
{
    public ValidationError(IEnumerable<ValidationFailure> errors)
        : base("Validation error")
    {
        var metadata = errors.ToDictionary(e => e.PropertyName, e => (object)e.ErrorMessage);
        WithMetadata(metadata);
    }
}