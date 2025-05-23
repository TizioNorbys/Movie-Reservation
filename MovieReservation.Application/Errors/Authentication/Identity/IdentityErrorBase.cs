using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace MovieReservation.Application.Errors.Authentication.Identity;

public abstract class IdentityErrorBase : Error
{
	protected IdentityErrorBase(string message, IEnumerable<IdentityError> errors)
        : base(message)
	{
        var metadata = errors.ToDictionary(e => e.Code, e => (object)e.Description);
        WithMetadata(metadata);
    }
}