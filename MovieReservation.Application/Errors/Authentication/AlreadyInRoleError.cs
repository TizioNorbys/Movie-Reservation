using FluentResults;

namespace MovieReservation.Application.Errors.Authentication;

public class AlreadyInRoleError : Error
{
	public AlreadyInRoleError(string roleName)
		: base($"User is already in {roleName} role")
	{
	}
}