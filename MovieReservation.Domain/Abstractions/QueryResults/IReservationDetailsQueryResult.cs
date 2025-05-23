namespace MovieReservation.Domain.Abstractions.QueryResults;

public interface IReservationDetailsQueryResult
{
    string Code { get; init; }
    string MovieTitle { get; init; }
    DateTime ShowtimeStart { get; init; }
    int HallNumber { get; init; }
    IEnumerable<int> SeatsNumber { get; init; }
}