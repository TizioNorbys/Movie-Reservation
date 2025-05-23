namespace MovieReservation.Application.DTOs.Showtime;

public record CreateShowtimeDto(DateTime Timestamp, Guid HallId, Guid MovieId) : ICreateUpdateShowtimeDto;