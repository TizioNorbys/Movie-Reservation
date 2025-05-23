namespace MovieReservation.Application.DTOs.Showtime;

public record UpdateShowtimeDto(DateTime Timestamp, Guid MovieId) : ICreateUpdateShowtimeDto;