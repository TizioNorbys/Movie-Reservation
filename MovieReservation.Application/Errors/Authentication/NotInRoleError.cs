using FluentResults;

namespace MovieReservation.Application.Errors.Authentication;

public class NotInRoleError : Error
{
    public NotInRoleError(string roleName)
        : base($"User is not in {roleName} role")
    {
    }
}