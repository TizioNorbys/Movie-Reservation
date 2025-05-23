using Microsoft.AspNetCore.Identity;
using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Domain.Entities;

public class User : IdentityUser<Guid>, IEntityBase
{
	public User(string email, string firstName, string lastName)
		: base(email)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(email);
		ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
		ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

		Email = email;
		FirstName = firstName;
		LastName = lastName;
	}

	public string FirstName { get; set; }
	public string LastName { get; set; }

	#region Navigation properties
	public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
	public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
	#endregion
}