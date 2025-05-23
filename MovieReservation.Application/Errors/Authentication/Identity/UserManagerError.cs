using Microsoft.AspNetCore.Identity;

namespace MovieReservation.Application.Errors.Authentication.Identity;

public class UserManagerError : IdentityErrorBase
{
    public UserManagerError(string message, IEnumerable<IdentityError> errors)
        : base(message, errors)
    {
    }
}