using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Domain.Entities;

public class Showtime : EntityBase
{
    public Showtime(DateTime timestamp, Guid hallId, Guid movieId)
    {
        Timestamp = timestamp;
        HallId = hallId;
        MovieId = movieId;
    }

    public Showtime()
    {
    }

    public DateTime Timestamp { get; set; }

    #region Navigation properties
    public Hall Hall { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
    public ICollection<Reservation> Reservations = new List<Reservation>();
    public ICollection<SeatReservation> SeatReservations = new List<SeatReservation>();
    #endregion

    #region Foreign keys
    public Guid HallId { get; set; }
    public Guid MovieId { get; set; }
    #endregion
}