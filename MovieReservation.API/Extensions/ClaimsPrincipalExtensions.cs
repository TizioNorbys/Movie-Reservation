using System.Security.Claims;

namespace MovieReservation.API.Extensions;

public static class ClaimsPrincipalExtensions
{
	public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
	{
		string? id = claimsPrincipal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

		return Guid.TryParse(id, out Guid userId) ? userId : throw new Exception("Couldn't get the user id from the principal");
    }
}