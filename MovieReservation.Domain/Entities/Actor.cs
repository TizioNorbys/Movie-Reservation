using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Domain.Entities;

public class Actor : EntityBase
{
	public Actor(string fullName, DateOnly birthDate)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

		FullName = fullName;
		BirthDate = birthDate;
	}

	public Actor(Guid id, string fullName, DateOnly birthDate)
		: this(fullName, birthDate) => Id = id;

	public Actor()
	{
	}

	public string FullName { get; set; } = null!;

	public DateOnly BirthDate { get; set; }

	public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}