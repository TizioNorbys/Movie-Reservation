using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Domain.Entities;

public class Hall : EntityBase
{
	public Hall(int number, int seatCount)
	{
		Number = number > 0 ? number : throw new ArgumentException("Hall number must be greater than 0");
        SeatCount = seatCount > 0 ? seatCount : throw new ArgumentException("Seat count must be greater than 0");
    }

	public int Number { get; set; }
	public int SeatCount { get; set; }

	public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
	public ICollection<Seat> Seats { get; set; } = new List<Seat>();
}