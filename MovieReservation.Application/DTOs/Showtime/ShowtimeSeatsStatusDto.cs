using MovieReservation.Domain.Abstractions.DTOs;
using MovieReservation.Domain.Abstractions.QueryResults;

namespace MovieReservation.Application.DTOs.Showtime;

public record ShowtimeSeatsStatusDto : IShowtimeSeatsQueryResult
{
    public required int AvailableSeats { get; init; }

    public required IEnumerable<ISeatStatusDto> SeatsStatus { get; init; }
}