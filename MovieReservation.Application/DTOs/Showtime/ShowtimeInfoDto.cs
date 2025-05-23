using MovieReservation.Domain.Abstractions.QueryResults;

namespace MovieReservation.Application.DTOs.Showtime;

public record ShowtimeInfoDto : IShowtimeInfoQueryResult
{
    public required DateTime Timestamp { get; init; }
}