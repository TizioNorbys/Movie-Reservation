using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Domain.Entities;

public class Reservation : EntityBase
{
	private Reservation(string code)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(code);

		Code = code;
	}

    public Reservation(string code, Guid userId, Guid showtimeId)
        : this(code) => (UserId, ShowtimeId) = (userId, showtimeId);

    public Reservation()
    {
    }

    public string Code { get; set; } = null!;

    #region Navigation properties
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();    // skip navigation
    public ICollection<SeatReservation> SeatReservations { get; set; } = new List<SeatReservation>();
    public User User { get; set; } = null!;
    public Showtime Showtime { get; set; } = null!;
    #endregion

    #region Foreign keys
    public Guid UserId { get; set; }
    public Guid ShowtimeId { get; set; }
    #endregion
}