using MovieReservation.Domain.Abstractions.QueryResults;

namespace MovieReservation.Application.DTOs.Reservation;

public record ReservationDetailsDto : IReservationDetailsQueryResult
{
    public required string Code { get; init; }

    public required string MovieTitle { get; init; }

    public required DateTime ShowtimeStart { get; init; }

    public required int HallNumber { get; init; }

    public required IEnumerable<int> SeatsNumber { get; init; }
}