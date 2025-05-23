using System.Text.Json.Serialization;
using MovieReservation.Domain.Constants;
using MovieReservation.Domain.Entities.Base;
using MovieReservation.Domain.Extensions;

namespace MovieReservation.Domain.Entities;

public class Movie : EntityBase
{
	public Movie(string title, string description, string plot, int releaseYear, string language, string country, int runningTime, double rating)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(plot);
		ArgumentOutOfRangeException.ThrowIfLessThan(rating, 1);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(rating, 10);
        language.ThrowIfNotDefined<Languages>(nameof(language));
		country.ThrowIfNotDefined<Countries>(nameof(country));

        Title = title;
		Description = description;
		Plot = plot;
		ReleaseYear = releaseYear; 
		OriginalLanguage = language; 
		Country = country; 
		RunningTime = runningTime; 
		Rating = rating; 
    }

	public Movie()
	{
	}

	public string Title { get; set; } = null!;
	public string Description { get; set; } = null!;
    public string Plot { get; set; } = null!;
    public int ReleaseYear { get; set; }
	public string OriginalLanguage { get; set; } = null!;
    public string Country { get; set; } = null!;
    public int RunningTime { get; set; }
	public double Rating { get; set; }

	#region Navigation properties
	public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
	[JsonIgnore]
	public ICollection<Genre> Genres { get; set; } = new List<Genre>();
	public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    #endregion
}