namespace MovieReservation.Domain.Abstractions.QueryResults;

public interface IShowtimeReservationQueryResult
{
    string Code { get; init; }
    string UserName { get; init; }
    IEnumerable<int> SeatsReserved { get; init; }
}