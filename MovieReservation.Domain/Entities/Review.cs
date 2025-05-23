using System.ComponentModel.DataAnnotations;
using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Domain.Entities;

public class Review : EntityBase
{
	private Review(int rating, string title, string content, DateTime timestamp)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(title);
		ArgumentException.ThrowIfNullOrWhiteSpace(content);

		if (rating < 1 || rating > 10)
			throw new ArgumentException("Rating must be between 1 and 10");

		Rating = rating;
		Title = title;
		Content = content;
		Timestamp = timestamp;
	}

	public Review(int rating, string title, string content, DateTime timestamp, Guid movieId, Guid userId)
		: this(rating, title, content, timestamp)
	{
		MovieId = movieId;
		UserId = userId;
	}

	public Review()
	{
	}

	public int Rating { get; set; }
	public string Title { get; set; } = null!;
	public string Content { get; set; } = null!;
	public DateTime Timestamp { get; set; }

	#region Navigation properties
	public Movie Movie { get; set; } = null!;
	public User User { get; set; } = null!;
	public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
	#endregion

	#region Foreign keys
	public Guid MovieId { get; set; }
	public Guid UserId { get; set; }
	#endregion
}