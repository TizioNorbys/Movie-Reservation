namespace MovieReservation.Domain.Entities;

public class SeatReservation
{
	public DateTime ReservedAt { get; set; }

	public Seat Seat { get; set; } = null!;
	public Reservation Reservation { get; set; } = null!;
	public Showtime Showtime { get; set; } = null!;

	public Guid SeatId { get; set; }
	public Guid ReservationId { get; set; }
	public Guid ShowtimeId { get; set; }
}