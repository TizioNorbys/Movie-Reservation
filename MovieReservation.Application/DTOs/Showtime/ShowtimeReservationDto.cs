using MovieReservation.Domain.Abstractions.QueryResults;

namespace MovieReservation.Application.DTOs.Showtime;

public record ShowtimeReservationDto
{
    public required string Code { get; init; }

    public required string UserName { get; init; }

    public required IEnumerable<int> SeatsReserved { get; init; }
}