using MovieReservation.Domain.Constants;
using MovieReservation.Domain.Entities.Base;
using MovieReservation.Domain.Extensions;

namespace MovieReservation.Domain.Entities;

public class Genre : EntityBase
{
	public Genre(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);
		name.ThrowIfNotDefined<MovieGenres>(nameof(Genre).ToLower());

		Name = name;
	}

    public string Name { get; set; }

	public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}