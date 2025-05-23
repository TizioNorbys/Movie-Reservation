using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Domain.Entities;

public class Seat : EntityBase
{
    public Seat(int number)
    {
        Number = number > 0 ? number : throw new ArgumentException("Seat number must be greater than 0");
    }

    public int Number { get; set; }

    #region Navigation properties
    public ICollection<Reservation> Reservation { get; set; } = new List<Reservation>();    // skip navigation
    public ICollection<SeatReservation> SeatReservations { get; set; } = new List<SeatReservation>();
    public Hall Hall { get; set; } = null!;
    #endregion

    #region Foreign keys
    public Guid HallId { get; set; }
    #endregion
}