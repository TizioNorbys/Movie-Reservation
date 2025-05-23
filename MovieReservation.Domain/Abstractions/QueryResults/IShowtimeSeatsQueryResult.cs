using MovieReservation.Domain.Abstractions.DTOs;

namespace MovieReservation.Domain.Abstractions.QueryResults;

public interface IShowtimeSeatsQueryResult
{
    int AvailableSeats { get; init; }
    IEnumerable<ISeatStatusDto> SeatsStatus { get; init; }
}