using Microsoft.AspNetCore.Identity;
using MovieReservation.Domain.Constants;
using MovieReservation.Domain.Entities.Base;
using MovieReservation.Domain.Extensions;

namespace MovieReservation.Domain.Entities;

public class Role : IdentityRole<Guid>, IEntityBase
{
	public Role(string name)
		: base(name)
	{
		name.ThrowIfNotDefined<RoleNames>($"{nameof(Role).ToLower()} name");
	}
}