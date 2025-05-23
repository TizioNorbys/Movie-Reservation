using Microsoft.AspNetCore.Identity;

namespace MovieReservation.Application.Errors.Authentication.Identity;

public class UserRolesUpdateError : IdentityErrorBase
{
	public UserRolesUpdateError(IEnumerable<IdentityError> errors)
		: base("Error while updating user roles", errors)
	{
	}
}