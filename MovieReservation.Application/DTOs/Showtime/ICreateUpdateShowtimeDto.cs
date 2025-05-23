namespace MovieReservation.Application.DTOs.Showtime;

public interface ICreateUpdateShowtimeDto
{
    DateTime Timestamp { get; init; }
    Guid MovieId { get; init; }
}