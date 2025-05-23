using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Domain.Entities;

public class Reaction : EntityBase
{
	public Reaction(bool type, DateTime timestamp, Guid reviewId, Guid userId)
	{
		Type = type;
		Timestamp = timestamp;
		ReviewId = reviewId;
		UserId = userId;
	}

	public bool Type { get; set; }
	public DateTime Timestamp { get; set; }

	#region Navigation properties
	public Review Review { get; set; } = null!;
	public User User { get; set; } = null!;
	#endregion

	#region Foreign keys
	public Guid ReviewId { get; set; }
	public Guid UserId { get; set; }
	#endregion
}