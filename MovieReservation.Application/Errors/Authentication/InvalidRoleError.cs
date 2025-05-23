using FluentResults;
using MovieReservation.Domain.Extensions;

namespace MovieReservation.Application.Errors.Authentication;

public class InvalidRoleError : Error
{
	public InvalidRoleError(string roleName)
		: base($"{roleName} is not a valid role")
	{
	}
}